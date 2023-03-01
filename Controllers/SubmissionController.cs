using Microsoft.AspNetCore.Mvc;
using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ticktrax_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SubmissionController : ControllerBase
{

    private readonly ILogger<SubmissionController> _logger;
    private ISubmissionService submissionService;
    private IUserService userService;

    private UserManager<User> userManager;

    public SubmissionController(ILogger<SubmissionController> logger, ISubmissionService submissionService, UserManager<User> userManager)
    {
        _logger = logger;
        this.submissionService = submissionService;
        this.userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await submissionService.Get();

        return new JsonResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIt(int id)
    {
        var result = await submissionService.GetById(id);

        if(result == null){
            return BadRequest("id does not exist");
        }

        return new JsonResult(result);
    }

    [HttpGet("closest")]
    public async Task<IActionResult> GetClosestLocation(double Longitude, double Latitude)
    {
        var result = await submissionService.GetByLocation(Longitude, Latitude);

        return new JsonResult(result);

    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Post([FromBody]SubmissionDto s)
    {
        if(!ModelState.IsValid){
            return BadRequest("garbage");
        }else{
            string currentUser = await GetCurrentUser();
            User usrname = await userService.GetUserById(currentUser);
            submissionService.AddSubmission(s,usrname);

            return Ok("posting...");
        }
    }

    public async Task<string> GetCurrentUser()
    {
        User currentUser = await GetCurrentUserAsync();
        return currentUser?.Id;
    }

    private Task<User> GetCurrentUserAsync() => userManager.GetUserAsync(HttpContext.User);

    
    


}

