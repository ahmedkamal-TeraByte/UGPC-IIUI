using System;
using System.ComponentModel.DataAnnotations;

namespace UGPC_IIUI.Models
{
    public class Project
    {
        public int ProjectId { get; set; }

        [Required]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime? SubmissionDate { get; set; }

        public string Status { get; set; }

        [Required]
        public string ProjectType { get; set; }

        [Required]
        public int GroupId { get; set; }
        public Group Group { get; set; }


    }
}