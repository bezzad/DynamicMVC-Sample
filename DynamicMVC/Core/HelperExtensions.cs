using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DynamicMVC.Core
{
    public static class HelperExtensions
    {
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
            {
                var obj = propertyDescriptor.GetValue(anonymousObject);
                expando.Add(propertyDescriptor.Name, obj);
            }

            return (ExpandoObject)expando;
        }


        public static dynamic DapperRowToExpando(this object value)
        {
            IDictionary<string, object> dapperRowProperties = value as IDictionary<string, object>;

            IDictionary<string, object> expando = new ExpandoObject();

            foreach (KeyValuePair<string, object> property in dapperRowProperties)
                expando.Add(property.Key, property.Value);

            return (ExpandoObject)expando;
        }


        public static DataTable ToDataTable(this IDataReader reader)
        {
            var dt = new DataTable();
            dt.Load(reader);

            return dt;
        }

        public static DataTable ToDataTable(this object[] data)
        {
            var dt = new DataTable();

            if (!data.Any()) return dt;

            // Get header
            var o = data[0] as IDictionary<string, object>;
            if (o == null) return dt;
            var dapperRowProperties = o.Keys;
            foreach (var col in dapperRowProperties)
            {
                dt.Columns.Add(col);
            }
            //
            // Get details
            foreach (IDictionary<string, object> item in data)
            {
                var row = dt.Rows.Add();
                foreach (string prop in dapperRowProperties)
                {
                    row[prop] = item[prop]?.ToString();
                }
            }

            return dt;
        }

        public static DataTable ToDataTable(this IEnumerable<dynamic> data)
        {
            return data.ToArray().ToDataTable();
        }


        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public static List<T> ToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

    }
}