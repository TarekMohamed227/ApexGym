using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApexGym.API.Filters;

// This class is an ACTION FILTER.
// It implements the 'IAsyncActionFilter' interface, which means ASP.NET Core
// will automatically call its 'OnActionExecutionAsync' method at the right time.
public class ValidationFilter : IAsyncActionFilter
{
    // This method is called by ASP.NET Core BEFORE your controller action runs.
    // 'context' contains information about the current request.
    // 'next' is a delegate that represents the next step in the pipeline (your controller action).
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // STEP 1: Check each parameter passed to the controller action.
        foreach (var argument in context.ActionArguments.Values)
        {
            // Skip if the argument is null.
            if (argument == null) continue;

            // Get the type of the object (e.g., 'MemberUpdateDto').
            var argumentType = argument.GetType();

            // Use REFLECTION to figure out what the Validator interface would be for this type.
            // For 'MemberUpdateDto', this creates the type 'IValidator<MemberUpdateDto>'.
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            // Now, ask the Dependency Injection (DI) container:
            // "Do you have a service that implements 'IValidator<MemberUpdateDto>'?"
            // 'GetService' returns null if it doesn't exist.
            var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            // If we found a validator for this type of object...
            if (validator != null)
            {
                // Create a validation context. This is just a wrapper around our object.
                var validationContext = new ValidationContext<object>(argument);

                // NOW, THE MOST IMPORTANT PART:
                // Run all the validation rules defined in the validator (e.g., MemberUpdateDtoValidator).
                // 'ValidateAsync' returns a 'ValidationResult' object.
                ValidationResult validationResult = await validator.ValidateAsync(validationContext);

                // THE 'ValidationResult' OBJECT:
                // This object has TWO main properties:
                // 1. 'IsValid' (bool): Is TRUE if all rules passed, FALSE if any rule failed.
                // 2. 'Errors' (List<ValidationFailure>): A list of all validation errors (if any).

                // Check if the validation failed.
                if (!validationResult.IsValid) // If 'IsValid' is false...
                {
                    // Create a simple list of error messages for the client.
                    var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });

                    // Set the result of the request to be a 400 Bad Request with the errors.
                    // This immediately SHORT-CIRCUITS the request. The controller action will NOT be called.
                    context.Result = new BadRequestObjectResult(errors);
                    return; // Exit the method early.
                }
            }
        }

        // STEP 2: If we get here, it means either:
        // a) There was no validator for the object, OR
        // b) The validator found no errors.
        // Therefore, we can proceed to call the actual controller action.
        await next(); // This calls the controller method (e.g., PutMember).
    }
}