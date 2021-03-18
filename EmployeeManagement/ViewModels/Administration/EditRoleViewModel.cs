using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels.Administration
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            UserNames = new List<string>();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is required.")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        public IEnumerable<string> UserNames { get; set; }
    }
}
