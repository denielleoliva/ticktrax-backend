using System.ComponentModel.DataAnnotations;
using Geolocation;
namespace ticktrax_backend.dtomodels
{
    public class SubmissionDto
    {
        //[Required]
        public string Photo { get; set; }

        //[Required]
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        //[Required]
        //[MaxLength(150)]
        //[MinLength(3)]
        public string Caption { get; set; }

        //[Required]
        public DateTime Time { get; set; }
    }
}