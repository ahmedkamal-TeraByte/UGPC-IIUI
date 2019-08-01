using System.ComponentModel.DataAnnotations;

namespace UGPC_IIUI.Models
{
    public class Student
    {
        public int StudentId { get; set; }


        [Required]
        public string Name { get; set; }

        [Display(Name = "Registration Number")]
        public int RegNo { get; set; }

        [Required]
        public string Batch { get; set; }

        [Required]
        public string Section { get; set; }

        [Display(Name = "Department")]
        public Department Department { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

    }
}