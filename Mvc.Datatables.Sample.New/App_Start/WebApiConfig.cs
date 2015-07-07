using Mvc.Datatables.Formatters;
using System.Web.Http;

namespace Mvc.Datatables.Sample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Add custom formatters
            config.Formatters.Insert(0, new DatatablesMediaTypeFormatter());
            config.Formatters.Insert(1, new DatatablesXmlMediaTypeFormatter());
        }
    }
}
