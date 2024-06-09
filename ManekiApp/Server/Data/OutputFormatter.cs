using Microsoft.AspNetCore.OData.Query.Wrapper;
using System.Reflection;
using System.Web;


namespace ManekiApp.Server.Data
{
    /// <summary>
    /// Class OutputFormatter.
    /// </summary>
    public static class OutputFormatter
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Object.</returns>
        public static object GetValue(object target, string name)
        {
            var selectExpandWrapper = target as ISelectExpandWrapper;

            return selectExpandWrapper != null ?
                selectExpandWrapper.ToDictionary()[name] :
                    target.GetType().GetProperty(name).GetValue(target);
        }

        /// <summary>
        /// Gets the properties from select.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;KeyValuePair&lt;System.String, Type&gt;&gt;.</returns>
        public static IEnumerable<KeyValuePair<string, Type>> GetPropertiesFromSelect(string queryString, Type type)
        {
            var select = HttpUtility.ParseQueryString(queryString)["$select"];
            var selectedPropertyNames = select != null ? select.Split(",") : new string[0];

            var elementType = typeof(ISelectExpandWrapper).IsAssignableFrom(type) ? type.GenericTypeArguments.First() : type;

            return GetProperties(elementType).Where(p => selectedPropertyNames.Contains(p.Key));
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;KeyValuePair&lt;System.String, Type&gt;&gt;.</returns>
        public static IEnumerable<KeyValuePair<string, Type>> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && OutputFormatter.IsSimpleType(p.PropertyType)).Select(p => new KeyValuePair<string, Type>(p.Name, p.PropertyType));
        }

        /// <summary>
        /// Determines whether [is simple type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is simple type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsSimpleType(Type type)
        {
            var underlyingType = type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                Nullable.GetUnderlyingType(type) : type;

            if (underlyingType == typeof(System.Guid) || underlyingType == typeof(System.DateTimeOffset))
                return true;

            var typeCode = Type.GetTypeCode(underlyingType);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }
    }
}
