using System.Collections.Generic;
using System.Security.Claims;

namespace EmployeeManagement.Models
{
    public static class Claims
    {
        public static List<Claim> All { get; set; } = new List<Claim>()
        {
            new Claim("Read", "Read"),
            new Claim("Edit", "Edit"),
            new Claim("Create", "Create"),
            new Claim("Delete", "Delete"),
        };
    }
}
