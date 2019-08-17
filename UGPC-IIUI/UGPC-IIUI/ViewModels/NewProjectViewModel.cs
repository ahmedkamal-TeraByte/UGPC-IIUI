using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using UGPC_IIUI.Models;

namespace UGPC_IIUI.ViewModels
{
    public class NewProjectViewModel
    {

        [Required]
        public string Title { get; set; }


        [DataType(DataType.Date)]
        [Required]
        public DateTime? SubmissionDate { get; set; }

        public string ProjectType { get; set; }

        public int GroupId { get; set; }

        public string Status { get; set; }
        public int ProjectId  { get; set; }


    [Required]
    [CustomValidation.PdfOnly(ErrorMessage = "Post PDF File only")]
    public HttpPostedFileBase ProjectFile { get; set; }

    public ApplicationUser Student1 { get; set; }
    public ApplicationUser Student2 { get; set; }


}
}