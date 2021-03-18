using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> employees;

        public MockEmployeeRepository()
        {
            this.employees = new List<Employee>()
            {
                new Employee() { Id = 1, Name = "Andrew", Email = "adashkov@borna-tech.com", Department = Department.Development },
                new Employee() { Id = 2, Name = "Sasha", Email = "sjabokrytski@borna-tech.com", Department = Department.Art },
                new Employee() { Id = 3, Name = "Marianna", Email = "mvysochanska@borna-tech.com", Department = Department.HR }
            };
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return this.employees.AsReadOnly();
        }

        public Employee GetEmployee(int id)
        {
            return this.employees.FirstOrDefault((employee) => employee.Id == id);
        }

        public Employee AddEmployee(Employee employee)
        {
            employee.Id = employees.Max((e) => e.Id) + 1;
            employees.Add(employee);
            return employee;
        }

        public Employee UpdateEmployee(Employee updatedEmployee)
        {
            Employee employee = employees.FirstOrDefault(e => e.Id == updatedEmployee.Id);

            if (employee != null)
            {
                employee.Name = updatedEmployee.Name ?? employee.Name;
                employee.Email = updatedEmployee.Email ?? employee.Email;
                employee.Department = updatedEmployee.Department ?? employee.Department;
            }

            return employee;
        }

        public Employee DeleteEmployee(int id)
        {
            Employee employee = employees.FirstOrDefault(e => e.Id == id);

            if (employee != null)
            {
                employees.Remove(employee);
            }

            return employee;
        }
    }
}
