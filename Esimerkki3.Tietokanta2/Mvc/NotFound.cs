using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Esimerkki3.Tietokanta2.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Esimerkki3.Tietokanta2.Mvc
{

    public class NotFound: ApiError {
        public NotFound() {
            StatusCode = StatusCodes.Status404NotFound;
        }
    }
    
    public class NotAuthorized: ApiError
    {
        public NotAuthorized() {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }

    public class Forbidden : ApiError
    {
        public Forbidden()
        {
            StatusCode = StatusCodes.Status403Forbidden;
        }
    }

    public class UndefinedError : ApiError
    {
        public UndefinedError()
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    // Validation structure closely follows Django validation logic, having unique code 
    // (not always provided unfortunately, e.g. on data annotations) allows to overwrite 
    // the message in the UI logic
    public class ValidationErrorMessage
    {
        public string Code { get; set; } = "";    // E.g. "MinLength", or "Required"
        public string Message { get; set; } = ""; // E.g. "This field is required"
        public object Data { get; set; } = "";    // E.g. { min: 5, max: 3 }
    }

    public class ValidationErrorData
    {
        public Dictionary<string, ValidationErrorMessage[]> Fields { get; set; } = new Dictionary<string, ValidationErrorMessage[]>();
        public ValidationErrorMessage[] General { get; set; } = new ValidationErrorMessage[] { };
    }
    
    public class ValidationError: ApiError<ValidationErrorData> {
         
        public ValidationError(ModelStateDictionary modelState)
        {
            StatusCode = StatusCodes.Status400BadRequest;
            JsonData = new ValidationErrorData()
            {
                Fields = modelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => new ValidationErrorMessage() {
                        Message = e.ErrorMessage
                    }).ToArray()
                )
            };
        }
        
        public ValidationError(IEnumerable<String> messages)
        {
            StatusCode = StatusCodes.Status400BadRequest;
            JsonData = new ValidationErrorData()
            {
                General = messages.Select(t => new ValidationErrorMessage()
                {
                    Message = t
                }).ToArray()
            };
        }

        public ValidationError(IEnumerable<ValidationErrorMessage> general = null, Dictionary<string, ValidationErrorMessage[]> fields = null)
        {
            StatusCode = StatusCodes.Status400BadRequest;
            JsonData = new ValidationErrorData()
            {
                General = (general ?? new List<ValidationErrorMessage>()).ToArray(),
                Fields = (fields ?? new Dictionary<string, ValidationErrorMessage[]>()),
            };
        }

        public ValidationError(String fieldName, ValidationErrorMessage message)
        {
            StatusCode = StatusCodes.Status400BadRequest;
            JsonData = new ValidationErrorData()
            {
                General = new List<ValidationErrorMessage>().ToArray(),
                Fields = new Dictionary<string, ValidationErrorMessage[]>()
                {
                    { fieldName, new ValidationErrorMessage[] { message } }
                },
            };
        }

        public ValidationError(String fieldName, IEnumerable<ValidationErrorMessage> messages)
        {
            StatusCode = StatusCodes.Status400BadRequest;
            JsonData = new ValidationErrorData()
            {
                General = new List<ValidationErrorMessage>().ToArray(),
                Fields = new Dictionary<string, ValidationErrorMessage[]>()
                {
                    { fieldName, messages.ToArray() }
                },
            };
        }
    }
}