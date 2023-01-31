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
    public IActionResult Post([FromBody]SubmissionDto s)
    {
        if(!ModelState.IsValid){
            return BadRequest("garbage");
        }else{
            submissionService.AddSubmission(s);
            return Ok("posting...");
        }
    }


}

