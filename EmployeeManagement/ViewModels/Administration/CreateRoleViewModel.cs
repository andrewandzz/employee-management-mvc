using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels.Administration
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
