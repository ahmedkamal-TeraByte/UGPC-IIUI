using System.ComponentModel.DataAnnotations;

namespace UGPC_IIUI.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Display(Name = "Student 1")]
        public string Student1Id { get; set; }

        public ApplicationUser Student1 { get; set; }

        [Display(Name = "Student 2")]
        public string Student2Id { get; set; }

        public ApplicationUser Student2 { get; set; }
    }
}