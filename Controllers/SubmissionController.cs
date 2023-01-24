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

    public IActionResult Get()
    {
        //gets posts submitted
        return Ok();
    }
    
    public IActionResult Get(int id)
    {
        //getting a specific post
        return Ok();
    }

    public IActionResult Post(Submission s)
    {
        //put a submission into database
        return Ok();
    }


}