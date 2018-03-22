using Microsoft.AspNetCore.Http;

namespace Esimerkki4.Kirjautuminen.Mvc
{
    public class Forbidden : ApiError
    {
        public Forbidden()
        {
            StatusCode = StatusCodes.Status403Forbidden;
        }
    }
}