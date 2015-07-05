using Mvc.Datatables.Formatters;
using Mvc.Datatables.Serialization;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
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

            // Replace default json converter
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.Add(new LegacyDatatablesMediaTypeFormatter());
            config.Formatters.Add(new XmlMediaTypeFormatter());
            config.Formatters.Add(new FormUrlEncodedMediaTypeFormatter());

            // Add a custom converter
            foreach (var formatter in GlobalConfiguration.Configuration.Formatters)
            {
                var jsonFormatter = formatter as JsonMediaTypeFormatter;
                if (jsonFormatter == null)
                    continue;

                jsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
                jsonFormatter.SerializerSettings.Converters.Add(new LegacyFilterRequestConverter());
                jsonFormatter.SerializerSettings.Converters.Add(new LegacyPageResponseConverter());
            }
        }
    }
}
