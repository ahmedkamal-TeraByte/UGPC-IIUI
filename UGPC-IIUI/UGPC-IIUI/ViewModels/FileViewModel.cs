using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UGPC_IIUI.ViewModels
{
    public class FileViewModel
    {
        public int ProjectId { get; set; }

        public string FileType { get; set; }


        [Required]
        [CustomValidation.PdfOnly(ErrorMessage = "Post PDF File only")]
        public HttpPostedFileBase ProjectFile { get; set; }
    }
}