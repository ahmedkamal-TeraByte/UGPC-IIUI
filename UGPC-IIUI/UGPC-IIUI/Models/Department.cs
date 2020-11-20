using System.ComponentModel.DataAnnotations;

namespace UGPC_IIUI.Models
{
    public class Department
    {

        public int DepartmentId { get; set; }

        [Display(Name = "Department")]
        public string Name { get; set; }
    }
}