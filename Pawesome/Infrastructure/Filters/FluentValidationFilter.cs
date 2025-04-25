using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Pawesome.Infrastructure.Filters;

/// <summary>
/// Action filter that automatically validates action parameters using FluentValidation
/// </summary>
/// <remarks>
/// This filter intercepts incoming requests and automatically validates any action parameters
/// that have corresponding FluentValidation validators registered in the service container.
/// For API controllers, it returns a BadRequest response with validation details.
/// For MVC controllers, it adds errors to ModelState to display them in views.
/// </remarks>
public class FluentValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    /// <summary>
    /// Initializes a new instance of the FluentValidationFilter class
    /// </summary>
    /// <param name="serviceProvider">Service provider used to resolve validators</param>
    /// <param name="problemDetailsFactory">Factory for creating ProblemDetails instances</param>
    public FluentValidationFilter(IServiceProvider serviceProvider, ProblemDetailsFactory problemDetailsFactory)
    {
        _serviceProvider = serviceProvider;
        _problemDetailsFactory = problemDetailsFactory;
    }

    /// <summary>
    /// Executes before the action method is invoked to validate parameters
    /// </summary>
    /// <param name="context">The context for the action being executed</param>
    /// <param name="next">The delegate to invoke the action method</param>
    /// <returns>A Task that on completion indicates the filter has executed</returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        bool isValid = true;

        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            if (context.ActionArguments.TryGetValue(parameter.Name, out var argumentValue) && argumentValue != null)
            {
                var argumentType = argumentValue.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    ValidationResult validationResult = await validator.ValidateAsync(
                        new ValidationContext<object>(argumentValue));

                    if (!validationResult.IsValid)
                    {
                        validationResult.AddToModelState(context.ModelState);
                        isValid = false;
                    }
                }
            }
        }

        if (!isValid)
        {

            if (context.Controller is Controller)
            {
                await next();
                return;
            }

            var problemDetails = _problemDetailsFactory.CreateValidationProblemDetails(
                context.HttpContext, context.ModelState);
            context.Result = new BadRequestObjectResult(problemDetails);
            return;
        }

        await next();
    }
}