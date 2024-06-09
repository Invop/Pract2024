using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace ManekiApp.Server.Data
{
    /// <summary>
    /// Class EntityPatch.
    /// </summary>
    public static class EntityPatch
    {
        /// <summary>
        /// Applies the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="source">The source.</param>
        public static void Apply(object obj, object source)
        {
            foreach (PropertyInfo property in obj.GetType().GetProperties().Where(p => p.CanWrite &&
                !p.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), false).Cast<DatabaseGeneratedAttribute>().Any()))
            {
                var value = property.GetValue(source, null);
                if (value != null)
                {
                    property.SetValue(obj, value, null);
                }
            }
        }


        /// <summary>
        /// Ifs the match.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>IDictionary&lt;System.String, System.Object&gt;.</returns>
        public static IDictionary<string, object> IfMatch(HttpRequest request, Type elementType)
        {
            StringValues ifMatchValues;
            if (request.Headers.TryGetValue("If-Match", out ifMatchValues))
            {
                var etagHeaderValue = EntityTagHeaderValue.Parse(ifMatchValues.SingleOrDefault());
                if (etagHeaderValue != null)
                {
                    var values = request
                        .GetETagHandler()
                        .ParseETag(etagHeaderValue) ?? new Dictionary<string, object>();

                    return elementType
                        .GetProperties()
                        .Where(pi => pi.GetCustomAttributes(typeof(ConcurrencyCheckAttribute), false).Any())
                        .OrderBy(pi => pi.Name)
                        .Select((pi, i) => new { Index = i, Name = pi.Name })
                        .ToDictionary(p => p.Name, p => values[p.Index.ToString()]);
                }
            }

            return null;
        }

        /// <summary>
        /// Applies to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        public static IQueryable<T> ApplyTo<T>(HttpRequest request, IQueryable<T> query)
        {
            var ifMatch = EntityPatch.IfMatch(request, query.ElementType);

            if (ifMatch != null)
            {
                var type = query.ElementType;
                var param = Expression.Parameter(type);
                Expression where = null;
                foreach (var item in ifMatch)
                {
                    var property = query.ElementType.GetProperty(item.Key);
                    var conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    var itemValue = (item.Value == null) ? null :
                        Convert.ChangeType(item.Value is DateTimeOffset ?
                            ((DateTimeOffset)item.Value).UtcDateTime : item.Value, conversionType);

                    var name = Expression.Property(param, item.Key);

                    var value = itemValue != null
                        ? IsNullable(property.PropertyType) ? ToNullable(Expression.Constant(itemValue)) : Expression.Constant(itemValue)
                        : Expression.Constant(value: null);

                    var equal = Expression.Equal(name, value);

                    where = where == null ? equal : Expression.AndAlso(where, equal);
                }

                if (where == null)
                {
                    return query;
                }

                return query.Where(Expression.Lambda<Func<T, bool>>(where, param));
            }

            return query;
        }

        /// <summary>
        /// Converts to nullable.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Expression.</returns>
        public static Expression ToNullable(Expression expression)
        {
            if (!IsNullable(expression.Type))
            {
                return Expression.Convert(expression, ToNullable(expression.Type));
            }

            return expression;
        }

        /// <summary>
        /// Determines whether the specified color type is nullable.
        /// </summary>
        /// <param name="clrType">Type of the color.</param>
        /// <returns><c>true</c> if the specified color type is nullable; otherwise, <c>false</c>.</returns>
        public static bool IsNullable(Type clrType)
        {
            if (clrType.IsValueType)
            {
                return clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Converts to nullable.
        /// </summary>
        /// <param name="clrType">Type of the color.</param>
        /// <returns>Type.</returns>
        public static Type ToNullable(Type clrType)
        {
            if (IsNullable(clrType))
            {
                return clrType;
            }
            else
            {
                return typeof(Nullable<>).MakeGenericType(clrType);
            }
        }
    }
}
