using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UGPC_IIUI.Models
{
    public class Change
    {
        public int  ChangeId { get; set; }
        public int ProjectId { get; set; }
        public string Changes { get; set; }
    }
}