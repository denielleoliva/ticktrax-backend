using System.ComponentModel.DataAnnotations;
namespace ticktrax_backend.dtomodels
{
    public class SubmissionDto
    {
        [Required]
        public string Photo { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [MaxLength(150)]
        [MinLength(3)]
        public string Caption { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }
    }
}