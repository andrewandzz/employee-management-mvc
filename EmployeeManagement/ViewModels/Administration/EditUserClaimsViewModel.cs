using System.Collections.Generic;

namespace EmployeeManagement.ViewModels.Administration
{
    public class EditUserClaimsViewModel
    {
        public EditUserClaimsViewModel()
        {
            Claims = new List<UserClaim>();
        }

        public string UserId { get; set; }
        public List<UserClaim> Claims { get; set; }
    }
}
