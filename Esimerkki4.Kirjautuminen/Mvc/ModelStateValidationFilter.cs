using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Esimerkki4.Kirjautuminen.Mvc
{
    public class ModelStateValidationFilter : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new ValidationError(context.ModelState).GetResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}