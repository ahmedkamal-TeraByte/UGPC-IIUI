﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UGPC_IIUI.Models;

namespace UGPC_IIUI.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string Name { get; set; }


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }



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


        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }


    }
}