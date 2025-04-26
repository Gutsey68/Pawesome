using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Pawesome.Infrastructure.Extensions;

/// <summary>
/// Extension methods for FluentValidation ValidationResult for better integration with ASP.NET Core
/// </summary>
/// <remarks>
/// These extensions simplify the conversion of FluentValidation validation results
/// to ASP.NET Core's ModelStateDictionary for consistent error handling in the application.
/// </remarks>
public static class ValidationExtensions
{
    /// <summary>
    /// Converts a FluentValidation ValidationResult to an ASP.NET Core ModelStateDictionary
    /// </summary>
    /// <param name="validationResult">The FluentValidation validation result to convert</param>
    /// <returns>A ModelStateDictionary containing all validation errors</returns>
    /// <remarks>
    /// This method maps each FluentValidation error to the corresponding field in ModelState,
    /// allowing validation errors to be seamlessly displayed in views using tag helpers.
    /// </remarks>
    public static ModelStateDictionary ToModelStateDictionary(this ValidationResult validationResult)
    {
        var modelState = new ModelStateDictionary();

        foreach (var error in validationResult.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return modelState;
    }
}