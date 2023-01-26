using ticktrax_backend.Models;
using System.Collections;
using ticktrax_backend.DataAnnotations;

public class SubmissionService : ISubmissionService
{

    private TickTraxContext context;

    public SubmissionService(TickTraxContext _ctx)
    {
        context = _ctx;
    }

    public async Task<bool> AddSubmission(Submission s)
    {
        Submission sub = new Submission
        {
            Photo = s.Photo,
            Location = s.Location,
            Caption = s.Caption,
            Time = s.Time
        };

        var result = await context.Submissions.AddAsync(sub);

        await context.SaveChangesAsync();

        if(result.IsKeySet)
        {
            return true;
        }

        return false;

    }

    public SubmissionService()
    {

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