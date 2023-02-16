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

    [HttpGet("{id?}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var result = await userService.GetUserById(id);
        
        if(result!=null)
        {
            return new JsonResult(result);
        }

        return BadRequest("user no exist");

    }

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

        return BadRequest("could not make a user");
    }

    private async Task<string> GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);


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

        if(newUser!=null)
        {
            await signInManager.PasswordSignInAsync(newUser, user.Password, false, false);
            var token = await GenerateToken(newUser);
            return Ok(token);
        }

        return NotFound("user not found");


    }

}