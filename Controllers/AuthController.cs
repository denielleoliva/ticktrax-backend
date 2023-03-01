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
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Cors;


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

    private readonly IConfiguration _config;

    public AuthController(ILogger<AuthController> logger, 
                                IUserService userService, 
                                UserManager<User> userManager, 
                                IEmailService emailService, 
                                SignInManager<User> signInManager,
                                IConfiguration configuration)
    {
        _logger = logger;
        this.userService = userService;
        this.userManager = userManager;
        this.emailService = emailService;
        this.signInManager = signInManager;
        _config = configuration;
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

    private async Task<string> GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        List<Claim> userClaims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };
        
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"], 
            _config["Jwt:Audience"], 
            userClaims, 
            DateTime.UtcNow, 
            DateTime.Now.AddDays(5), 
            credentials);


        return new JwtSecurityTokenHandler().WriteToken(token);
        
    }

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

    public async Task<IActionResult> AddRole(string email, string roleName)
    {
        var user = await userManager.FindByEmailAsync(email);

        if(user!=null)
        {
            var result = await userManager.AddToRoleAsync(user, roleName);

            if(result.Succeeded)
            {
                _logger.LogInformation(1, $"User {user.Email} added to the {roleName} role");
                return Ok(new {result = $"User {user.Email} added to the {roleName} role"});
            }else
            {
                _logger.LogInformation (1, $"Error: Unable to add user {user.Email} to the {roleName} role");
                return BadRequest(new {error = $"Error: Unable to add user {user.Email} to the {roleName} role"});
            }

           
        }
    
        return BadRequest(new {error = $"Error: Unable to find user"});

    }

    public async Task<IActionResult> GetUserRoles(string email)
    {
        var user = await userManager.FindByEmailAsync(email);

        var roles = await userManager.GetRolesAsync(user);

        return Ok(roles);
    }

    public async Task<IActionResult> RemoveRole(string email, string roleName)
    {

        var user = await userManager.FindByEmailAsync(email);

        if(user != null)
        {
            var result = await userManager.RemoveFromRoleAsync(user, roleName);

            if(result.Succeeded)
            {
                _logger.LogInformation (1, $"User {user.Email} removed from the {roleName} role");
                return Ok(new {result = $"User {user.Email} removed from the {roleName} role"});
            }
            else
            {
                _logger.LogInformation (1, $"Error: Unable to removed user {user.Email} from the {roleName} role");
                return BadRequest(new {error = $"Error: Unable to removed user {user.Email} from the {roleName} role"});
            }
        }

            // User doesn't exist
        return BadRequest(new {error = "Unable to find user"});

    }






}