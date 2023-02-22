using System.ComponentModel.DataAnnotations;

namespace ticktrax_backend.Models
{

    public class AuthRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

    }

}