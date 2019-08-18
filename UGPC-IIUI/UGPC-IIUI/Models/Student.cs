using System.ComponentModel.DataAnnotations;

namespace UGPC_IIUI.Models
{
    public class Student
    {
        public int StudentId { get; set; }


        [Required]
        [Display(Name = "Registration Number")]
        public int RegNo { get; set; }

        [Required]
        public string Batch { get; set; }

        [Required]
        public string Section { get; set; }

        public bool CanSubmitProposal { get; set; }
    }
}