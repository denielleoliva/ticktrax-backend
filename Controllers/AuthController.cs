using Microsoft.AspNetCore.Mvc;
using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Cors;
using ticktrax_backend.Data;


namespace ticktrax_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;
    private IUserService userService;
    private UserManager<User> userManager;
    private IEmailService emailService;
    private SignInManager<User> signInManager;
    private TickTraxContext _context;

    private readonly IConfiguration _config;


    public AuthController(ILogger<AuthController> logger, 
                                IUserService userService, 
                                UserManager<User> userManager, 
                                IEmailService emailService, 
                                SignInManager<User> signInManager,
                                IConfiguration configuration,
                                TickTraxContext context)
    {
        _logger = logger;
        this.userService = userService;
        this.userManager = userManager;
        this.emailService = emailService;
        this.signInManager = signInManager;
        _config = configuration;
        _context = context;
    }


    //param: string id
    //output: string user
    //description: get a user based on a user id from request
    [HttpGet("{id?}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var result = await userService.GetUserById(id);
        
        if(result!=null)
        {
            return new JsonResult(result);
        }

        return BadRequest("User does not exist");

    }

    [HttpGet("/user/{username?}")]
    public async Task<IActionResult> GetUserByName(string userName)
    {
        var result = await userService.GetUserByUserName(userName);
        
        if(result!=null)
        {
            return new JsonResult(result);
        }

        return BadRequest("User does not exist");

    }

    [HttpGet("/email/{email?}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var result = await userService.GetUserByEmail(email);
        
        if(result!=null)
        {
            return new JsonResult(result);
        }

        return BadRequest("User does not exist");

    }


    [HttpGet("/user/{username?}/profilephoto")]
    public async Task<IActionResult> GetUserPhoto(string username)
    {
        var result = await userService.GetUserByUserName(username);

        if(result!=null)
        {
            if(result.ProfilePhoto!=null)
            {
                return new JsonResult(result.ProfilePhoto);
            }else
            {
                return BadRequest("no profile photo associated with user");
            }
        }

        return BadRequest("User not found");
    }

    //param: UserDto user
    //output: result userCreated
    //description: create a new user with username, email, and password
    //  password should have: 1 uppercase, 1 number, 1 special character
    //  email should be a valid email
    //  username should be unique
    [EnableCors]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserDto user)
    {
        // if(userService.GetUserByEmail(user.Email)==null)
        // {
        //     return BadRequest("UserName or Email already in use");
        // }

        var result = await userService.AddUser(user);

        if(result.Succeeded)
        {
            User newUser = await userService.GetUserByEmail(user.Email);


            _logger.LogInformation("User created new account with password.");

            // var code = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
            // code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            // var callbackUrl = Url.Page(
            //     "/Account/ConfirmEmail",
            //     pageHandler: null,
            //     values: new { area = "Identity", userId = newUser.Id, code = code },
            //     protocol: Request.Scheme);

            // emailService.SendConfirmationEmailAsync(user.Email, "sendemail" + callbackUrl);

            return new JsonResult(newUser);

        }

        //If user was not created check if password meets criteria (1 uppercase, 1 number, 1 character)
        return BadRequest("could not make a user");
    }

    [HttpPost("/update/{id}")]
    public  async Task<IActionResult> UpdateUserInformation([FromBody]UserDto user, string id)
    {
        User? userToEdit = await userManager.FindByIdAsync(id);

        if(userToEdit.UserName != user.UserName){
            userToEdit.UserName = user.UserName;
        }

        if(userToEdit.Email != user.Email){
            userToEdit.Email = user.Email;
        }

        if(userToEdit.FirstName != user.FirstName){
            userToEdit.FirstName = user.FirstName;
        }

        if(userToEdit.LastName != user.LastName){
            userToEdit.LastName = user.LastName;
        }

        IdentityResult result = await userManager.UpdateAsync(userToEdit);

        await _context.SaveChangesAsync();


        if(result.Succeeded)
        {
            return Ok("user updated");
        }


        return BadRequest("user not updated");
    }


    //param: User user
    //output: Jwt token created
    //description: generate a token for user authentication on login
    private async Task<string> GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> userClaims = new List<Claim> {
            new Claim("UserName", user.UserName),
            new Claim("Email", user.Email),
        };

        var userRoles = await userManager.GetRolesAsync(user);

        if(userRoles.Count != 0)
        {
            userClaims.Add(new Claim(ClaimTypes.Role, userRoles.First()));
        }
        
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"], 
            _config["Jwt:Audience"], 
            userClaims, 
            DateTime.UtcNow, 
            DateTime.Now.AddDays(5), 
            credentials);


        return new JwtSecurityTokenHandler().WriteToken(token);
        
    }


    //param: UserDto user
    //output: user signed in
    //description: check user credentials and login user if token is valid
    [HttpPost("signin")]
    public async Task<IActionResult> SignInUser([FromBody]UserDto user)
    {
        User newUser = null;

        if(user.Email!=null)
        {
            newUser = await userService.GetUserByEmail(user.Email);
        }else if(user.UserName!=null){
            newUser = await userService.GetUserByUserName(user.UserName);
        }

        var result = userManager
                            .PasswordHasher
                            .VerifyHashedPassword(newUser, 
                                                    newUser.PasswordHash, 
                                                    user.Password);

        if(newUser!=null && userManager
                            .PasswordHasher
                            .VerifyHashedPassword(newUser, 
                                                    newUser.PasswordHash, 
                                                    user.Password) != PasswordVerificationResult.Failed)
        {
            await signInManager.PasswordSignInAsync(newUser, user.Password, false, false);
            var token = await GenerateToken(newUser);
            ClaimsPrincipal userClaims = new ClaimsPrincipal();
            HttpContext.User = await signInManager.CreateUserPrincipalAsync(newUser);
            return new JsonResult(token);
        }

        return NotFound("user not found");


    }

    [HttpGet("signout")]
    public async Task<IActionResult> SignOut()
    {
        await signInManager.SignOutAsync();

        return Ok("signing out...");
    }


}