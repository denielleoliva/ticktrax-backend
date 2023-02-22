using Microsoft.AspNetCore.Identity;
using ticktrax_backend.Models;

public class AuthResponse: IdentityUser
{

    public string UserName { get; set; }

    public string Password { get; set; }

    public string Token { get; set; }
    

}