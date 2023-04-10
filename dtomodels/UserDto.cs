using System.Security.Claims;
using Microsoft.AspNetCore.Identity;


namespace ticktrax_backend.dtomodels{

    public class UserDto{

        public string UserName { get; set;}

        public string Email { get; set; }

        public string Password { get; set; }

        public byte[] ProfilePhoto { get; set; }

        

    }

}