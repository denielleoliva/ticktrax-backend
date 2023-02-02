using Microsoft.AspNetCore.Identity;
using ticktrax_backend.Models;

public class User: IdentityUser
{

    public IEnumerable<Submission> UserSubmissions { get; set; }
    

}