using System;
using System.ComponentModel.DataAnnotations;

namespace UGPC_IIUI.ViewModels
{
    public class CreatePresentationsViewModel
    {

        [Required]
        [Display(Name = "No. Of Presentations")]
        public int NumberOfPresentations { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }
    }
}