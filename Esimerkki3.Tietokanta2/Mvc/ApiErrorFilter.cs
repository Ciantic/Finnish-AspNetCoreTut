using System;
using Esimerkki3.Tietokanta2.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Esimerkki3.Tietokanta2.Mvc
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