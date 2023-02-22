using System.ComponentModel.DataAnnotations;

namespace ticktrax_backend.Models
{

    public class AuthRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime Expiration { get; set; }

    }

}