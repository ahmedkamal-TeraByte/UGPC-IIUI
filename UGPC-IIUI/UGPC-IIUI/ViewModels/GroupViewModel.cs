using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UGPC_IIUI.Models;

namespace UGPC_IIUI.ViewModels
{
    public class GroupViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
        public int GroupId { get; set; }

        [Display(Name = "Student 1")]
        public string Student1Id { get; set; }
        public ApplicationUser Student1 { get; set; }

        [Display(Name = "Student 2")]
        public string Student2Id { get; set; }

        public ApplicationUser Student2 { get; set; }
    }
}