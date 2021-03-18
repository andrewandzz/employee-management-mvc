using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels.Home
{
    public class CreateViewModel
    {
        [Required]
        [MaxLength(30, ErrorMessage = "Name cannot be more than 30 characters long.")]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email format.")]
        [Display(Name = "Work Email")]
        public string Email { get; set; }

        [Required]
        public Department? Department { get; set; }

        public IFormFile Photo { get; set; }
    }
}
