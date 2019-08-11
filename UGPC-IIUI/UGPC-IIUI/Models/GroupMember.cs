using System.ComponentModel.DataAnnotations;

namespace UGPC_IIUI.Models
{
    public class GroupMember
    {
        [Key]
        public int GroupMembershipId { get; set; }
        [Required]
        public int GroupId { get; set; }

        public Group Group { get; set; }

        public string Student1Id { get; set; }

        public ApplicationUser Student1 { get; set; }

        public string Student2Id { get; set; }

        public ApplicationUser Student2 { get; set; }
    }
}