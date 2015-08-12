using Mvc.Datatables.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Mvc.Datatables.Formatters
{
    public class DatatablesMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public static readonly string ApplicationDatatablesMediaType = "application/json+datatables";
        public static readonly string TextDatatablesMediaType = "text/json+datatables";

        public DatatablesMediaTypeFormatter()
            : base()
        {
            this.SupportedMediaTypes.Clear();
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue(ApplicationDatatablesMediaType));
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue(TextDatatablesMediaType));

            this.MediaTypeMappings.Clear();
            this.AddRequestHeaderMapping("x-requested-with", "XMLHttpRequest", StringComparison.OrdinalIgnoreCase, true, ApplicationDatatablesMediaType);

            this.SerializerSettings.Converters.Add(new FilterRequestConverter());
            this.SerializerSettings.Converters.Add(new PageResponseConverter());
            this.SerializerSettings.Converters.Add(new StringEnumConverter());

            this.SerializerSettings.Formatting = Formatting.Indented;
            this.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public new static MediaTypeHeaderValue DefaultMediaType
        {
            get
            {
                return new MediaTypeHeaderValue(ApplicationDatatablesMediaType);
            }
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
        }

        public override bool CanReadType(Type type)
        {
            foreach (var converter in this.SerializerSettings.Converters)
                if (converter.GetType() != typeof(StringEnumConverter) && converter.CanConvert(type))
                    return true;

            return false;
        }

        public override bool CanWriteType(Type type)
        {
            foreach (var converter in this.SerializerSettings.Converters)
                if (converter.GetType() != typeof(StringEnumConverter) && converter.CanConvert(type))
                    return true;

            return false;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var task = Task<object>.Factory.StartNew(() =>
            {
                var sr = new StreamReader(readStream);
                var jreader = new JsonTextReader(sr);

                var ser = JsonSerializer.Create(this.SerializerSettings);

                object val = ser.Deserialize(jreader, type);
                return val;
            });

            return task;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() =>
            {
                string json = JsonConvert.SerializeObject(value, this.SerializerSettings);

                byte[] buf = Encoding.Default.GetBytes(json);
                writeStream.Write(buf, 0, buf.Length);
                writeStream.Flush();
            });

            return task;
        }
    }
}
