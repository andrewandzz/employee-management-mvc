using System.Collections.Generic;

namespace EmployeeManagement.ViewModels.Administration
{
    public class EditUsersInRoleViewModel
    {
        public EditUsersInRoleViewModel()
        {
            Users = new List<UserInRoleViewModel>();
        }

        public string RoleId { get; set; }
        public List<UserInRoleViewModel> Users { get; set; }
    }
}
