using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UGPC_IIUI.Models
{
    public class Student
    {
        public int StudentID { get; set; }
        [Required]
        public string Name { get; set; }

        [Display(Name = "Registration Number")]
        public int RegNo { get; set; }
        [Required]
        public string Batch { get; set; }
        [Required]
        public string Section { get; set; }

        [Display (Name = "Department")]
        public Department Department { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public enum Department
    {
        CS,
        SE,
        IT

    }
}