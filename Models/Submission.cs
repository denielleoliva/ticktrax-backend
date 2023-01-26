using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ticktrax_backend.Models
{
    public class Submission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Photo { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Caption { get; set; }

        [Required]
        public DateTime Time { get; set; }
    }
}