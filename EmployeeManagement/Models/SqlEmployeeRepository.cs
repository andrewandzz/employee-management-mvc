using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;

namespace EmployeeManagement.Models
{
    public class SqlEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext context;

        public SqlEmployeeRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Employee AddEmployee(Employee employee)
        {
            this.context.Employees.Add(employee);
            this.context.SaveChanges();
            return employee;
        }

        public Employee DeleteEmployee(int id)
        {
            Employee employee = this.context.Employees.Find(id);

            if (employee != null)
            {
                this.context.Employees.Remove(employee);
                this.context.SaveChanges();
            }

            return employee;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return this.context.Employees;
        }

        public Employee GetEmployee(int id)
        {
            return this.context.Employees.Find(id);
        }

        public Employee UpdateEmployee(Employee updatedEmployee)
        {
            EntityEntry<Employee> employeeEntry = this.context.Employees.Attach(updatedEmployee);
            employeeEntry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this.context.SaveChanges();

            return updatedEmployee;
        }
    }
}
