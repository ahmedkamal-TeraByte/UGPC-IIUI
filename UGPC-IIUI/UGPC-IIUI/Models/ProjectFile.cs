using System.ComponentModel.DataAnnotations;

namespace UGPC_IIUI.Models
{
    public class ProjectFile
    {
        public int ProjectFileId { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Required]
        public string FilePath { get; set; }

        public string FileType { get; set; } //either proposal or something else like presentation etc
        public string FileName { get; set; }
    }
}