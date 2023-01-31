using Microsoft.EntityFrameworkCore;
using ticktrax_backend.Models;
using System.Collections;
using ticktrax_backend.Data;
using ticktrax_backend.dtomodels;
using Geolocation;

public class SubmissionService : ISubmissionService
{


    private TickTraxContext context;

    public SubmissionService(TickTraxContext _ctx)
    {
        context = _ctx;
    }

    public async Task<bool> AddSubmission(SubmissionDto sub)
    {
        Submission s = new Submission
        {
            Photo = sub.Photo,
            Latitude = sub.Latitude,
            Longitude = sub.Longitude,
            Caption = sub.Caption,
            Time = sub.Time
        };
        var result = await context.Submissions.AddAsync(s);
        context.SaveChanges();

        if (result.IsKeySet)
        {
            return true;
        }

        return false;
    }

    public async Task<Submission> DeleteSubmission(int id)
    {
        var item = await context.Submissions.Where(task => task.Id == id).FirstOrDefaultAsync();

        if(item != null)
        {
            var returns = context.Submissions.Remove(item);
            return returns.Entity;
        }

        return item;
    }

    public async Task<Submission> UpdateSubmission(Submission sub)
    {
        var subToUpdate = await context.Submissions.Where(submissionItem => submissionItem.Photo == sub.Photo && submissionItem.Time == sub.Time).FirstOrDefaultAsync();

        if(subToUpdate != null)
        {
            subToUpdate.Photo = sub.Photo;
            subToUpdate.Latitude = sub.Latitude;
            subToUpdate.Longitude = sub.Longitude;
            subToUpdate.Caption = sub.Caption;
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
        var result = await context.Submissions.Where(sub => sub.Id == id).FirstOrDefaultAsync();

        //fix nullable at some point
        return result;
    }

    public async Task<Submission> GetByLocation(double Longitude, double Latitude)
    {
       var result = await context.Submissions.Select(x => new { x, delta = Math.Abs(x.Latitude - Latitude) + Math.Abs(x.Longitude - Longitude)})
        .OrderBy(x => x.delta)
        .FirstOrDefaultAsync();

        //fix nullable at some point
        return result.x;
    }

    public Task<bool> Post(Submission s)
    {


        throw new NotImplementedException();
    }

    


}