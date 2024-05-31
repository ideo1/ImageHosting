using ImageHosting.Services.ImageService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ImageHosting.Filters
{
    public class ImageProcessAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        private const int bearerLength = 2;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"];

            if (!authorizationHeader.ToString().StartsWith("Bearer"))
            {
                context.Result = new UnauthorizedResult();

                return;
            }

            var bearerHeader = authorizationHeader.ToString().Split(' ');

            if (bearerHeader == null || bearerHeader.Length != bearerLength)
            {
                context.Result = new UnauthorizedResult();

                return;
            }

            var settings = context.HttpContext.RequestServices.GetService<ImageServiceConfiguration>();

            if (bearerHeader.LastOrDefault() != settings.Authorize.Token)
            {
                return;
            }
        }
    }
}
