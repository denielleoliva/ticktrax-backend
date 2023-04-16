using Microsoft.AspNetCore.Mvc;
using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;
using ticktrax_backend.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using System.IO;
using System.Data;
using System.Text;
using System.ComponentModel;
using Microsoft.AspNetCore.Cors;
using System.Data;
using Microsoft.VisualBasic.FileIO;

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
            //await DownloadImage(s.Photo, "/Users/denielleoliva/Documents/"+fileName);
            await DownloadImage(s.Photo, "../photos_ticktrax/" + fileName);

            GetTickPrediction(s.FileName);

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
        byte [] imageRead = System.IO.File.ReadAllBytes("../photos_ticktrax/"+fileName);
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

    [HttpGet("/prediction/{photoName}")]
    public async Task<IActionResult> ReturnPrediction(string photoName)
    {
        return new JsonResult(GetTickPrediction(photoName));
    }

    public string GetTickPrediction(string fileName)
    {
        DataTable tickPredictionTable = new DataTable();

        string csvFilePath = "~/ticktrax/Desktop/results.csv";

        try
        {
            using(TextFieldParser csvReader = new TextFieldParser(csvFilePath))
            {
                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;
                string[] colFields = csvReader.ReadFields();

                foreach (string column in colFields)
                {
                    DataColumn datacolumn = new DataColumn(column);
                    datacolumn.AllowDBNull = true;
                    tickPredictionTable.Columns.Add(datacolumn);
                }

                while (!csvReader.EndOfData)
                {
                    string[] fieldData = csvReader.ReadFields();
                    //Making empty value as null
                    for (int i = 0; i < fieldData.Length; i++)
                    {
                        if (fieldData[i] == "")
                        {
                            fieldData[i] = null;
                        }
                    }

                    tickPredictionTable.Rows.Add(fieldData);
                }
            }
        }catch(Exception e){

        }

        DataRow dr = tickPredictionTable.AsEnumerable()
               .SingleOrDefault(r=> r.Field<string>("FNAME") == fileName);

        return dr.Field<string>("GENUS")+","+dr.Field<string>("SPECIES");
    }


    //--------------------------TESTING (NOT WORKING YET)----------------------------------

    // [HttpGet("submissionreport")]
    // [Authorize(AuthenticationSchemes = "Bearer")]
    // public IActionResult DownloadCsv()
    // {
    //     PropertyDescriptorCollection properties = 
    //         TypeDescriptor.GetProperties(typeof(Submission));

    //     DataTable table = new DataTable();

    //     foreach(PropertyDescriptor prop in properties)
    //     {
    //         table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
    //     }

    //     foreach(Submission submission in context.Submissions)
    //     {
    //         DataRow row = table.NewRow();
    //         foreach(PropertyDescriptor prop in properties)
    //         {
    //             row[prop.Name] = prop.GetValue(submission) ?? DBNull.Value;
    //         }
    //         table.Rows.Add(row);
    //     }

    //     return File(ExportData(table), "text/csv");
    // }

    // public static string ExportData(DataTable table)
    // {
    //     StringBuilder csvData = new StringBuilder();

    //     foreach(var col in table.Columns)
    //     {
    //         csvData.Append(col.ToString() + ",");
    //     }

    //     csvData.Replace(",", System.Environment.NewLine, csvData.Length - 1, 1);

    //     foreach(DataRow row in table.Rows)
    //     {
    //         foreach(var col in row.ItemArray)
    //         {
    //             csvData.Append("\"" + col.ToString() + "\",");
    //         }

    //         csvData.Replace("," , System.Environment.NewLine, csvData.Length - 1, 1);
    //     }

    //     return csvData.ToString();
    // }



    
    


}

