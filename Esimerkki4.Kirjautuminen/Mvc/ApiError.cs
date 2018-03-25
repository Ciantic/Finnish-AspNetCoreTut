using System;
using Esimerkki4.Kirjautuminen.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Esimerkki4.Kirjautuminen.Mvc
{
    public class ErrorValue<T>
    {
        public string Error { get; set; } = "";
        public T Data { get; set; }
    }

    abstract public class ApiError : Exception
    {
        public int? StatusCode { get; set; }

        virtual public IActionResult GetResult()
        {
            return new ObjectResult(new ErrorValue<object>()
            {
                Error = GetType().Name,
                Data = null
            })
            {
                StatusCode = StatusCode
            };
        }
    }
    
    abstract public class ApiError<T> : ApiError
        where 
            T : class, new()
    {
        public T JsonData { get; set; }

        override public IActionResult GetResult()
        {
            return new ObjectResult(new ErrorValue<T>()
            {
                Error = GetType().Name,
                Data = JsonData
            })
            {
                StatusCode = StatusCode
            };
        }
    }
}