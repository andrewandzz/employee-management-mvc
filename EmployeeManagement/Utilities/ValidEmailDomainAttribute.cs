using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string requiredDomain;

        public ValidEmailDomainAttribute(string domain)
        {
            this.requiredDomain = domain;
        }

        public override bool IsValid(object email)
        {
            string[] parts = email.ToString().Split('@');

            if (parts.Length != 2)
            {
                throw new ArgumentException("Provided email string is invalid.");
            }

            string domain = parts[1];

            return this.requiredDomain.ToUpper() == domain.ToUpper();
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-email-domain", ErrorMessage);
            context.Attributes.Add("data-val-email-domain-value", requiredDomain);
        }
    }
}
