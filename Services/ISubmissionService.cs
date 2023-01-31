using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;
using Geolocation;

public interface ISubmissionService
{
    Task<bool> AddSubmission(SubmissionDto sub);

    Task<Submission> DeleteSubmission(int id);

    Task<Submission> UpdateSubmission(Submission sub);

    Task<IEnumerable<Submission>> Get();

    Task<Submission> GetById(int id);

     Task<Submission> GetByLocation(double Longitude, double Latitude);

    Task<bool> Post(Submission s);
}