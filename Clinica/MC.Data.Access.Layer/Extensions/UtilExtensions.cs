using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace MC.Data.Access.Layer.Extensions
{
    public static class UtilExtensions
    {
        private const string EspacioEnBlanco = " ";
        public static T GetEntity<T>(this DbDataReader dataReader)
        {
            var properties = typeof(T).GetProperties();
            T item = default;
            var columns = dataReader.GetColumnSchema();

            while (dataReader.Read())
            {
                item = (T)Activator.CreateInstance(typeof(T));
                foreach (var (property, value) in GetPropertiesWithValues(dataReader, properties, columns))
                {
                    property.SetValue(item, value);
                }
            }

            return item;
        }


        public static List<T> GetEntities<T>(this DbDataReader dataReader)
        {
            var properties = typeof(T).GetProperties();
            var columns = dataReader.GetColumnSchema();
            List<T> entities = new();
            while (dataReader.Read())
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                foreach (var (property, value) in GetPropertiesWithValues(dataReader, properties, columns))
                {
                    property.SetValue(item, value);
                }
                entities.Add(item);
            }

            return entities;
        }

        public static List<T> NextResult<T>(this DbDataReader dataReader)
        {
            dataReader.NextResult();
            return dataReader.GetEntities<T>();
        }

        public static T NextResultEntity<T>(this DbDataReader dataReader)
        {
            dataReader.NextResult();
            return dataReader.GetEntity<T>();
        }

        public static async Task<List<T>> GetEntitiesAsync<T>(this DbDataReader dataReader)
        {
            var properties = typeof(T).GetProperties();
            var columns = await dataReader.GetColumnSchemaAsync();
            List<T> entities = new();
            while (await dataReader.ReadAsync())
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                foreach (var (property, value) in GetPropertiesWithValues(dataReader, properties, columns))
                {
                    property.SetValue(item, value);
                }
                entities.Add(item);
            }

            return entities;
        }

        public static string ConvertToJson(this object data)
        {
            return JsonSerializer.Serialize(data);
        }


        private static IEnumerable<(PropertyInfo property, object value)>
           GetPropertiesWithValues(IDataRecord dataReader,
           PropertyInfo[] properties,
           IEnumerable<DbColumn> columns)
        {
            return from property in properties
                   where columns.Any(c => c.ColumnName.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                   let value = dataReader[property.Name]
                   where value != DBNull.Value
                   select (property, value);
        }

        public static string ToJson<T>(this IEnumerable<T> list)
        {
            var json = JsonSerializer.Serialize(list);
            return json;
        }


        public static string    ToStringParameter(this string valor)
        {
            string valorParametro = valor?.Trim();
            if (string.IsNullOrWhiteSpace(valorParametro))
            {
                return EspacioEnBlanco;
            }
            else
            {
                return valor;
            }
        }

    }
}
