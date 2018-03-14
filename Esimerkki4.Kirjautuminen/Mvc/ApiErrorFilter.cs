using System;
using Esimerkki4.Kirjautuminen.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Esimerkki4.Kirjautuminen.Mvc
{
    public class ApiErrorFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is NotFoundException)
            {
                context.Result = new NotFound().GetResult();
                context.Exception = null; // Nollaa virhe
                context.ExceptionHandled = true; // Virhe käsitelty
            }
            // Voit käsitellä tässä filtterissä muitakin exceptioneita
        }
    }
}