using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Ajax.Utilities;

namespace NewISE.Models.ViewModel
{
    internal class NoValidateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }
    }
}