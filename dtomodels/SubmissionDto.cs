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
        public DateTime Time { get; set; }

        //Required
        public string OwnerId { get; set;}
    }
}