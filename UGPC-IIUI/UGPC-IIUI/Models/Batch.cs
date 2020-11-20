using System.ComponentModel.DataAnnotations;

namespace UGPC_IIUI.Models
{
    public class Batch
    {

        [Display(Name = "Batch Name")]
        public int BatchId { get; set; }

        [Required]
        [Display(Name = "Batch Name")]
        public string Name { get; set; }
    }
}