using System.Collections.Generic;

namespace Esimerkki4.Kirjautuminen.Mvc
{
    public class ValidationErrorData
    {
        public Dictionary<string, ValidationErrorMessage[]> Fields { get; set; } = new Dictionary<string, ValidationErrorMessage[]>();
        public ValidationErrorMessage[] General { get; set; } = new ValidationErrorMessage[] { };
    }
}