using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Esimerkki4.Kirjautuminen.Mvc
{
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