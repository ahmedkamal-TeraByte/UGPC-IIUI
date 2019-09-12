using System.ComponentModel.DataAnnotations;

namespace UGPC_IIUI.Models
{
    public class Section
    {
        public int SectionId { get; set; }

        [Required]
        [Display(Name = "Batch")]
        public int BatchId { get; set; }
        [Display(Name = "Section Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Batch")]
        public Batch Batch { get; set; }
    }

}