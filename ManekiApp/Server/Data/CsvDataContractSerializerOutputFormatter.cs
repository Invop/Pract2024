using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;

namespace ManekiApp.Server.Data
{
    /// <summary>
    /// Class CsvDataContractSerializerOutputFormatter.
    /// Implements the <see cref="TextOutputFormatter" />
    /// </summary>
    /// <seealso cref="TextOutputFormatter" />
    public class CsvDataContractSerializerOutputFormatter : TextOutputFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataContractSerializerOutputFormatter"/> class.
        /// </summary>
        public CsvDataContractSerializerOutputFormatter()
        {
            SupportedMediaTypes.Add("text/csv");
            SupportedEncodings.Add(Encoding.Unicode);
        }

        /// <summary>
        /// Writes the response body.
        /// </summary>
        /// <param name="context">The formatter context associated with the call.</param>
        /// <param name="selectedEncoding">The <see cref="T:System.Text.Encoding" /> that should be used to write the response.</param>
        /// <returns>A task which can write the response body.</returns>
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var query = (IQueryable)context.Object;

            var queryString = context.HttpContext.Request.QueryString;
            var columns = queryString.Value.Contains("$select") ?
                OutputFormatter.GetPropertiesFromSelect(queryString.Value, query.ElementType) :
                    OutputFormatter.GetProperties(query.ElementType);

            var sb = new StringBuilder();

            foreach (var item in query)
            {
                var row = new List<string>();

                foreach (var column in columns)
                {
                    var value = OutputFormatter.GetValue(item, column.Key);

                    row.Add($"{value}".Trim());
                }

                sb.AppendLine(string.Join(",", row.ToArray()));
            }

            return context.HttpContext.Response.WriteAsync(
                $"{string.Join(",", columns.Select(c => c.Key))}{System.Environment.NewLine}{sb.ToString()}",
                selectedEncoding,
                context.HttpContext.RequestAborted);
        }
    }
}
