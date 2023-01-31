using Microsoft.AspNetCore.Mvc;
using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;

namespace ticktrax_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SubmissionController : ControllerBase
{

    private readonly ILogger<SubmissionController> _logger;
    private ISubmissionService submissionService;

    public SubmissionController(ILogger<SubmissionController> logger, ISubmissionService submissionService)
    {
        _logger = logger;
        this.submissionService = submissionService;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        //gets posts submitted
        return Ok("you got me");
    }

    [HttpGet("{id}")]
    public IActionResult GetIt(int id)
    {
        //getting a specific post
        return Ok(id);
    }

    [HttpPost]
    public IActionResult Post([FromBody]SubmissionDto s)
    {
        if(!ModelState.IsValid){
            return BadRequest("garbage");
        }else{
            Console.WriteLine(s.Photo);
            submissionService.AddSubmission(s);
            return Ok();
        }
    }


}

