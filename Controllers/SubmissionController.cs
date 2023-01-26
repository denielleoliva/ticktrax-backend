using Microsoft.AspNetCore.Mvc;
using ticktrax_backend.Models;

namespace ticktrax_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SubmissionController : ControllerBase
{

    private readonly ILogger<SubmissionController> _logger;

    public SubmissionController(ILogger<SubmissionController> logger)
    {
        _logger = logger;
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

    public IActionResult Post(Submission s)
    {
        //put a submission into database
        return Ok();
    }


}

