using System.Collections.Generic;
using UGPC_IIUI.Models;

namespace UGPC_IIUI.ViewModels
{
    public class CommitteeViewModel
    {

        public Committee Committee { get; set; }

        public Dictionary<ApplicationUser, string> CommitteeMembers = new Dictionary<ApplicationUser, string>();

        public ApplicationUser Member { get; set; }

        public string Role { get; set; }
        public int committeeId { get; set; }
    }
}