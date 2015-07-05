using Mvc.Datatables.Serialization;
using System.Web.Mvc;

namespace Mvc.Datatables.Sample
{
    public class BinderConfig
    {
        public static void RegisterBinders(ModelBinderDictionary modelBinderDictionary)
        {
            modelBinderDictionary.Add(typeof (FilterRequest), new FilterRequestModelBinder());
        }
    }
}
