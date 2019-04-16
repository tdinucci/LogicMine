using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogicMine.Trace.Json
{
    public class JsonLoggerBasket : LoggerBasket
    {
        internal JsonLoggerBasket()
        {
        }

        public JsonLoggerBasket(string service, IBasket basket) : base(service, basket)
        {
        }

        protected override string SerialiseObject(object obj)
        {
            if (obj == null)
                return string.Empty;

            var jObject = JObject.FromObject(obj, new JsonSerializer {Formatting = Formatting.Indented});
            ClearUnloggableProperties(obj.GetType(), jObject);

            return jObject.ToString();
        }

        private void ClearUnloggableProperties(Type type, JObject jobj)
        {
            foreach (var prop in type.GetProperties())
            {
                if (prop.GetCustomAttribute<NoLogAttribute>() != null)
                    jobj[prop.Name] = null;
            }
        }
    }
}