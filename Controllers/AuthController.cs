using Microsoft.AspNetCore.Mvc;
using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
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

    public AuthController(ILogger<AuthController> logger, 
                                IUserService userService, 
                                UserManager<User> userManager, 
                                IEmailService emailService, 
                                SignInManager<User> signInManager)
    {
        _logger = logger;
        this.userService = userService;
        this.userManager = userManager;
        this.emailService = emailService;
        this.signInManager = signInManager;
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

}