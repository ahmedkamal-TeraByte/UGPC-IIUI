using System.ComponentModel.DataAnnotations;
using UGPC_IIUI.Models;

namespace UGPC_IIUI.ViewModels
{
    public class CommitteeMemberViewModel
    {
        public ApplicationUser Member { get; set; }

        [Display(Name = "Member Name")]
        public string MemberId { get; set; }

        public int ProfessorId { get; set; }
        public int CommitteeId { get; set; }
        public string Role { get; set; }
    }
}