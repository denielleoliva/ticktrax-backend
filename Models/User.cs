using Microsoft.AspNetCore.Identity;
using ticktrax_backend.Models;

public class User: IdentityUser
{

    public IEnumerable<Submission> UserSubmissions { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public byte[] ProfilePhoto { get; set; }

}