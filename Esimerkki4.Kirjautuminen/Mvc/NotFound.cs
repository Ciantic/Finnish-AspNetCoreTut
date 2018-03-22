using System.Text.RegularExpressions;
using Esimerkki4.Kirjautuminen.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Esimerkki4.Kirjautuminen.Mvc
{

    public class NotFound: ApiError {
        public NotFound() {
            StatusCode = StatusCodes.Status404NotFound;
        }
    }
}