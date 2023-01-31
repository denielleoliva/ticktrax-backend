using Microsoft.EntityFrameworkCore;
using ticktrax_backend.Models;
using System.Collections;
using ticktrax_backend.Data;
using ticktrax_backend.dtomodels;

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
            Location = sub.Location,
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
            subToUpdate.Location = sub.Location;
            subToUpdate.Caption = sub.Caption;
            subToUpdate.Time = sub.Time;

            context.Submissions.Update(subToUpdate);
        }else{
            throw new System.Exception("no models found to update");
        }

        return subToUpdate;
    }

    public Task<IEnumerable<Submission>> Get()
    {

        throw new NotImplementedException();
    }

    public Task<Submission> Get(int id)
    {


        throw new NotImplementedException();
    }

    public Task<bool> Post(Submission s)
    {


        throw new NotImplementedException();
    }

    


}