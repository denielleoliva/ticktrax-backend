using ticktrax_backend.Models;

public interface ISubmissionService
{
    Task<IEnumerable<Submission>> Get();

    Task<Submission> Get(int id);

    Task<bool> Post(Submission s);
}