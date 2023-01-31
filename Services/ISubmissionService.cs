using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;

public interface ISubmissionService
{
    Task<bool> AddSubmission(SubmissionDto sub);

    Task<Submission> DeleteSubmission(int id);

    Task<Submission> UpdateSubmission(Submission sub);

    Task<IEnumerable<Submission>> Get();

    Task<Submission> Get(int id);

    Task<bool> Post(Submission s);
}