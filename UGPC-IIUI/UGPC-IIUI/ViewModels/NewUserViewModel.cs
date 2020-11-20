using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UGPC_IIUI.Models;

namespace UGPC_IIUI.ViewModels
{
    public class NewUserViewModel
    {
        [Required]
        public string Name { get; set; }


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public IEnumerable<Department> Departments { get; set; }

        public Student Student { get; set; }

        public Professor Professor { get; set; }

        public string userId { get; set; }


        [Display(Name = "Role Name")]
        public string Role { get; set; }


    }
}