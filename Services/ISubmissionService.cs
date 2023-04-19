using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;
using Geolocation;

public interface ISubmissionService
{
    Task<bool> AddSubmission(SubmissionDto sub, User currentUser);

    Task<bool> DeleteSubmission(int id);

    Task<Submission> UpdateSubmission(Submission sub);

    Task<IEnumerable<Submission>> Get();

    Task<Submission> GetById(int id);

    Task<Submission> GetByLocation(double Longitude, double Latitude);

    IEnumerable<string> GetTickPrediction(string fileName);

}