using System.Security.Claims;
using Microsoft.AspNetCore.Identity;


namespace ticktrax_backend.dtomodels{

    public class UserDto{

        public string UserName { get; set;}

        public string Email { get; set; }

        public string Password { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public IEnumerable<Claim> Claims()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, UserName) };

            claims.AddRange(Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims;
        }

    }

}