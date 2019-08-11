using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UGPC_IIUI.Models
{
    public class CommitteeMember
    {

        [Key]
        [Column(Order = 1)]
        public int CommitteeId { get; set; }
        public Committee Committee { get; set; }

        [Key]
        [Column(Order = 2)]

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}