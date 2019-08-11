using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Tt.HttpUtils
{
    public static class Conversions
    {
        public static string ToQueryString(this object anonymous)
        {
            if (anonymous == null)
            {
                return string.Empty;
            }

            var properties = anonymous.GetType().GetProperties();

            if (!properties.Any())
            {
                return string.Empty;
            }

            var parameters = new List<Parameter>();
            foreach (var property in properties)
            {
                var value = property.GetValue(anonymous, null);

                if (property.PropertyType.IsArray)
                {
                    var array = (Array)value;

                    if (array == null) continue;

                    foreach (var arrayElement in array)
                    {
                        parameters.Add(new Parameter(property.Name, arrayElement));
                    }
                }
                else
                {
                    parameters.Add(new Parameter(property.Name, value));
                }
            }

            var items = new StringBuilder("?");
            var isFirst = true;

            foreach (var kvp in parameters)
            {
                if (!isFirst)
                {
                    items.Append("&");
                }

                if (kvp.Value is DateTime)
                {
                    items.AppendFormat("{0}={1}", kvp.Name, ((DateTime)kvp.Value).ToString("O", CultureInfo.InvariantCulture));
                }
                else
                {
                    items.AppendFormat("{0}={1}", kvp.Name, kvp.Value);
                }

                isFirst = false;
            }

            return items.ToString();
        }

        private class Parameter
        {
            public Parameter(string name, object value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; private set; }
            public object Value { get; private set; }
        }
    }
}