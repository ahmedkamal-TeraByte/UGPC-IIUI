using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace UGPC_IIUI.Models
{
    public class Professor
    {
        public int ProfessorId { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }
    }

    public enum Role
    {
        Coordinator,
        Supervisor,
        CoSupervisor
    }
}