using Microsoft.EntityFrameworkCore;
using ticktrax_backend.Models;
using System.Collections;
using ticktrax_backend.Data;
using ticktrax_backend.dtomodels;
using Geolocation;
using System.Diagnostics;

public class SubmissionService : ISubmissionService
{


    private TickTraxContext context;

    
    public SubmissionService(TickTraxContext _ctx)
    {
        context = _ctx;
    }

    public async Task<bool> AddSubmission(SubmissionDto sub, User currentUser)
    {
        Submission s = new Submission
        {
            Photo = sub.Photo,
            FileName = sub.FileName,
            FileType = sub.FileType,
            Latitude = sub.Latitude,
            Longitude = sub.Longitude,
            Time = sub.Time,
            OwnerId = currentUser.Id
        };
        var result = await context.Submissions.AddAsync(s);
        context.SaveChanges();

        if (result.IsKeySet)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteSubmission(int id)
    {
        Submission? item = await context.Submissions.Where(task => task.Id == id).FirstOrDefaultAsync();

        if(item != null)
        {
            var returns = context.Submissions.Remove(item);
            return true;
        }

        return false;
    }

    public async Task<Submission> UpdateSubmission(Submission sub)
    {
        Submission? subToUpdate = await context.Submissions.Where(submissionItem => submissionItem.Photo == sub.Photo && submissionItem.Time == sub.Time).FirstOrDefaultAsync();

        if(subToUpdate != null)
        {
            subToUpdate.Photo = sub.Photo;
            subToUpdate.FileName = sub.FileName;
            subToUpdate.FileType = sub.FileType;
            subToUpdate.Latitude = sub.Latitude;
            subToUpdate.Longitude = sub.Longitude;
            subToUpdate.Time = sub.Time;

            context.Submissions.Update(subToUpdate);
        }else{
            throw new System.Exception("no models found to update");
        }

        return subToUpdate;
    }

    public async Task<IEnumerable<Submission>> Get()
    {
        return await context.Submissions.ToListAsync();
    }

    public async Task<Submission> GetById(int id)
    {
        Submission? result = await context.Submissions.Where(sub => sub.Id == id).FirstOrDefaultAsync();

        //fix nullable at some point
        return result;
    }

    public IEnumerable<string> GetTickPrediction(string fileName)
    {

        string csvFilePath = "../Desktop/results.csv";

        var lineAsArray = System.IO.File.ReadLines(csvFilePath).Where(s => s.Contains(fileName));

        Console.WriteLine(lineAsArray);

        return lineAsArray;
    }
    
    private void run_cmd()
    {
        string source = "../home/ticktrax/photos_ticktrax/";
        string dest = "../home/ticktrax/Desktop";
        string model = "../home/ticktrax/Desktop";

        string cmd = "predict.py";
        string args = source + dest + model;

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "../home/ticktrax/Desktop/TickIDNet-main/predict.py";
        start.Arguments = string.Format("{0} {1}", cmd, args);
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        using(Process process = Process.Start(start))
        {
            using(StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                Console.Write(result);
            }
        }
    }


    //Source: https://github.com/scottschluer/geolocation
    public async Task<Submission> GetByLocation(double Longitude, double Latitude)
    {
       var result = await context.Submissions.Select(x => new { x, delta = Math.Abs(x.Latitude - Latitude) + Math.Abs(x.Longitude - Longitude)})
        .OrderBy(x => x.delta)
        .FirstOrDefaultAsync();

        //fix nullable at some point
        return result.x;
    }



    


}