using System;
using System.ComponentModel.DataAnnotations;

namespace UGPC_IIUI.Models
{
    public class Presentation
    {
        public int PresentationId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [DataType(DataType.Time)]
        public DateTime? Time { get; set; }

        public string Status { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }
    }
}