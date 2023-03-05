using Microsoft.AspNetCore.Mvc;
using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using System.IO;

namespace ticktrax_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SubmissionController : ControllerBase
{

    private readonly ILogger<SubmissionController> _logger;
    private ISubmissionService submissionService;
    private IUserService userService;

    private UserManager<User> userManager;

    //param: 
    //output: constructor
    //description: dependency injection
    public SubmissionController(ILogger<SubmissionController> logger, 
        ISubmissionService submissionService, 
        UserManager<User> userManager, 
        IUserService userService)
    {
        _logger = logger;
        this.submissionService = submissionService;
        this.userService = userService;
        this.userManager = userManager;
    }
    

    //param: 
    //output: IActionResult list
    //description: return a list of all the submissions in the database
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await submissionService.Get();

        return new JsonResult(result);
    }


    //param: int id
    //output: IActionResult submission
    //description: get a specific submission with an id
    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetIt(int id)
    {
        var result = await submissionService.GetById(id);

        if(result == null){
            return BadRequest("id does not exist");
        }

        return new JsonResult(result);
    }


    //param: double longitude double latitude
    //output: IActionResult location
    //description: returns the closest location based on a given "pin" on map
    [HttpGet("closest")]
    public async Task<IActionResult> GetClosestLocation(double Longitude, double Latitude)
    {
        var result = await submissionService.GetByLocation(Longitude, Latitude);

        return new JsonResult(result);

    }


    //param: submissiondto s
    //output: IActionResult submission
    //description: check if the submission format is valid
    //  add submission to database if it's valid
    //  download image to the server
    //  require auth jwt token to post
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Post([FromBody]SubmissionDto s)
    {
        string fileName = s.FileName + "." + s.FileType;

        if(!ModelState.IsValid){
            return BadRequest("garbage");
        }else{

            string user = HttpContext.User.Claims.First(c => c.Type == "UserName").Value;
            User currentUser = await userService.GetUserByUserName(user);
            await submissionService.AddSubmission(s,currentUser);
            await DownloadImage(s.Photo, "/Users/denielleoliva/Documents/"+fileName);
            //await DownloadImage(s.Photo, "../photos_ticktrax/imgtest.png");

            return Ok("saved post...");
        }
    }


    //param: string fileName
    //output: IActionResult photo
    //description: get a photo from the database to the frontend
    //  return an image based on a filename
    [HttpGet("{fileName}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public IActionResult GetUserPhoto(string fileName)
    {

        byte [] imageRead = System.IO.File.ReadAllBytes("/Users/denielleoliva/Documents/"+fileName);
        return File(imageRead, "image/png");

    }


    //param: string imageBase64 string filePath
    //output: bool fileAdded
    //description: image is downloaded from the base64 string in the json file
    //  converted from the base64 string from the http response
    //  image should be saved on the file path
    public async Task<bool> DownloadImage(byte[] img, string filePath)
    {

        System.IO.File.WriteAllBytes(filePath, img);

        return true;

    }



    
    


}

