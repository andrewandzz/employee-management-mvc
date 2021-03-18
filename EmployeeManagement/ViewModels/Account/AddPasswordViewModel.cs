using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels.Account
{
    public class AddPasswordViewModel
    {
        [Required]
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")]
        [Display(Name = "Confirm new password")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
