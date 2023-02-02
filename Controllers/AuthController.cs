using Microsoft.AspNetCore.Mvc;
using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

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

}