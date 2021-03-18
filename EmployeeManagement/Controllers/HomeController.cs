using EmployeeManagement.Models;
using EmployeeManagement.Security;
using EmployeeManagement.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IDataProtector dataProtector;

        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment hostEnvironment, IDataProtectionProvider dataProtectionProvider)
        {
            this.employeeRepository = employeeRepository;
            this.hostEnvironment = hostEnvironment;

            dataProtector = dataProtectionProvider
                .CreateProtector(DataProtectionPurposeStrings.EmployeeIdRouteValue);
        }

        public ViewResult Index()
        {
            IEnumerable<Employee> model = employeeRepository.GetAllEmployees().Select(employee =>
            {
                employee.EncryptedId = dataProtector.Protect(employee.Id.ToString());
                return employee;
            });

            return View(model);
        }

        [Route("home/details/{encriptedId}")]
        public ViewResult Details(string encriptedId)
        {
            int employeeId = Convert.ToInt32(dataProtector.Unprotect(encriptedId));

            Employee employee = employeeRepository.GetEmployee(employeeId);

            if (employee == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return View("EmployeeNotFound", employeeId);
            }

            DetailsViewModel model = new DetailsViewModel()
            {
                PageTitle = "Employee Details",
                Employee = employee
            };

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string fileName = null;

                if (model.Photo != null)
                {
                    fileName = ProcessUploadedPhoto(model.Photo);
                }

                Employee newEmployee = new Employee()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoFileName = fileName
                };

                Employee addedEmployee = this.employeeRepository.AddEmployee(newEmployee);

                return RedirectToAction("details", new { id = addedEmployee.Id });
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public ViewResult Edit(int id)
        {
            Employee employee = this.employeeRepository.GetEmployee(id);
            EditViewModel model = new EditViewModel()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                CurrentPhotoFileName = employee.PhotoFileName
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee editedEmployee = new Employee()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoFileName = model.CurrentPhotoFileName
                };

                if (model.Photo != null)
                {
                    editedEmployee.PhotoFileName = ProcessUploadedPhoto(model.Photo);

                    if (model.CurrentPhotoFileName != null)
                    {
                        DeletePhotoFile(model.CurrentPhotoFileName);
                    }
                }

                this.employeeRepository.UpdateEmployee(editedEmployee);

                return RedirectToAction("index");
            }

            return View();
        }

        private string ProcessUploadedPhoto(IFormFile photo)
        {
            string uploadDirectory = Path.Combine(hostEnvironment.WebRootPath, "images");
            string fileName = $"{Guid.NewGuid()}_{photo.FileName}";
            string photoPath = Path.Combine(uploadDirectory, fileName);

            using FileStream stream = new FileStream(photoPath, FileMode.Create);

            photo.CopyTo(stream);

            return fileName;
        }

        private void DeletePhotoFile(string fileName)
        {
            string fullFilePath = Path.Combine(hostEnvironment.WebRootPath, "images", fileName);
            System.IO.File.Delete(fullFilePath);
        }
    }
}
