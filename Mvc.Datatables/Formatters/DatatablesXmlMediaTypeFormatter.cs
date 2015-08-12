using Mvc.Datatables.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Mvc.Datatables.Formatters
{
    public class DatatablesXmlMediaTypeFormatter : MediaTypeFormatter
    {
        public static readonly string ApplicationDatatablesMediaType = "application/xml+datatables";
        public static readonly string TextDatatablesMediaType = "text/xml+datatables";

        private readonly ICollection<Type> _knownTypes;
        private readonly XmlReaderSettings _readerSettings;
        private readonly XmlWriterSettings _writerSettings;

        public DatatablesXmlMediaTypeFormatter()
            : this(null, null, null)
        { }

        public DatatablesXmlMediaTypeFormatter(ICollection<Type> knownTypes)
            : this(knownTypes, null, null)
        { }

        public DatatablesXmlMediaTypeFormatter(XmlReaderSettings readerSettings, XmlWriterSettings writerSettings)
            : this(null, readerSettings, writerSettings)
        { }

        public DatatablesXmlMediaTypeFormatter(ICollection<Type> knownTypes, XmlReaderSettings readerSettings, XmlWriterSettings writerSettings)
            : base()
        {
            this.SupportedMediaTypes.Clear();
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue(ApplicationDatatablesMediaType));
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue(TextDatatablesMediaType));

            _knownTypes = knownTypes ?? new List<Type>();
            _readerSettings = readerSettings ?? new XmlReaderSettings();
            _writerSettings = writerSettings ?? new XmlWriterSettings();
        }

        public static MediaTypeHeaderValue DefaultMediaType
        {
            get
            {
                return new MediaTypeHeaderValue(ApplicationDatatablesMediaType);
            }
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(IFilterRequest) || type.IsSubclassOf(typeof(IFilterRequest))
                || type == typeof(IPageResponse) || type.IsSubclassOf(typeof(IPageResponse));
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(IFilterRequest) || type.IsSubclassOf(typeof(IFilterRequest))
                || type == typeof(IPageResponse) || type.IsSubclassOf(typeof(IPageResponse));
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var task = Task<object>.Factory.StartNew(() =>
            {
                if (type == typeof(IPageResponse) || type.IsSubclassOf(typeof(IPageResponse)))
                {
                    IPageResponse message = Activator.CreateInstance(type) as IPageResponse;
                    Type dataSourceType = message.GetPageResponseArgument();
                    if (dataSourceType != null && !_knownTypes.Contains(dataSourceType))
                        _knownTypes.Add(dataSourceType);
                }

                XmlReader reader = XmlReader.Create(readStream, _readerSettings);
                XmlSerializer ser = new XmlSerializer(type, _knownTypes.ToArray());
                object val = ser.Deserialize(reader);
                return val;
            });

            return task;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() =>
            {
                if (value != null)
                {
                    Type objectType = value.GetType();
                    if (objectType == typeof(IPageResponse) || objectType.IsSubclassOf(typeof(IPageResponse)))
                    {
                        Type dataSourceType = ((IPageResponse)value).GetPageResponseArgument();
                        if (dataSourceType != null && !_knownTypes.Contains(dataSourceType))
                            _knownTypes.Add(dataSourceType);
                    }

                    XmlWriter writer = XmlWriter.Create(writeStream, _writerSettings);
                    XmlSerializer ser = new XmlSerializer(type, _knownTypes.ToArray());
                    ser.Serialize(writer, value);
                }
            });

            return task;
        }
    }
}
