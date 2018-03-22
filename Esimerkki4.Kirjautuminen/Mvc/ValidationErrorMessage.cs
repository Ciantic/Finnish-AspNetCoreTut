namespace Esimerkki4.Kirjautuminen.Mvc
{
    // Validation structure closely follows Django validation logic, having unique code 
    // (not always provided unfortunately, e.g. on data annotations) allows to overwrite 
    // the message in the UI logic
    public class ValidationErrorMessage
    {
        public string Code { get; set; } = "";    // E.g. "MinLength", or "Required"
        public string Message { get; set; } = ""; // E.g. "This field is required"
        public object Data { get; set; } = "";    // E.g. { min: 5, max: 3 }
    }
}