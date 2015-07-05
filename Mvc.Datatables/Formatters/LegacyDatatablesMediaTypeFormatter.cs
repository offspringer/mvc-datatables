using Mvc.Datatables.Serialization;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mvc.Datatables.Formatters
{
	public class LegacyDatatablesMediaTypeFormatter : JsonMediaTypeFormatter
	{
		public static readonly string ApplicationDatatablesMediaType = "application/datatables";
		public static readonly string TextDatatablesMediaType = "text/datatables";

        public LegacyDatatablesMediaTypeFormatter()
			: base()
		{
			this.SupportedMediaTypes.Clear();
			this.SupportedMediaTypes.Add(new MediaTypeHeaderValue(ApplicationDatatablesMediaType));
			this.SupportedMediaTypes.Add(new MediaTypeHeaderValue(TextDatatablesMediaType));

			this.MediaTypeMappings.Clear();
			this.AddRequestHeaderMapping("x-requested-with", "XMLHttpRequest", StringComparison.OrdinalIgnoreCase, true, ApplicationDatatablesMediaType);

            this.SerializerSettings.Converters.Add(new LegacyFilterRequestConverter());
            this.SerializerSettings.Converters.Add(new LegacyPageResponseConverter());

            this.SerializerSettings.Formatting = Formatting.Indented;
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
			return true;
		}

		public override bool CanWriteType(Type type)
		{
			return true;
		}

		public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
		{
			return base.ReadFromStreamAsync(type, readStream, content, formatterLogger);
		}

		public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
		{
			return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
		}
	}
}
