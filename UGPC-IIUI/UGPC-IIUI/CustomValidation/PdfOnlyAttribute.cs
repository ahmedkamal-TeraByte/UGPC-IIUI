using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace UGPC_IIUI.CustomValidation
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PdfOnlyAttribute : ValidationAttribute
    {
        private string AllowedExtensions { get; set; }

        public PdfOnlyAttribute()
        {
            AllowedExtensions = ".pdf";
        }

        public override bool IsValid(object value)
        {
            if (!(value is HttpPostedFileBase file)) return true;
            var fileName = file.FileName;

            return fileName.EndsWith(AllowedExtensions);

        }
    }
}