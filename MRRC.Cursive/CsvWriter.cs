using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MRRC.Cursive
{
    /// <summary>
    /// Writes data to a CSV file
    /// </summary>
    public class CsvWriter
    {
        private readonly struct Header
        {
            public string Name { get; }
            public PropertyInfo Property { get; }

            public Header(PropertyInfo property)
            {
                Property = property;
                Name = property.GetCustomAttribute<NameAttribute>()?.Name ?? property.Name;
            }
        }
        
        private readonly TextWriter _writer;
        private readonly CultureInfo _culture;

        public CsvWriter(TextWriter writer, CultureInfo culture)
        {
            _writer = writer;
            _culture = culture;
        }

        /// <summary>
        /// Writes the specified records (including headers) to the CSV file
        /// </summary>
        public void WriteRecords<T>(IEnumerable<T> records)
        {
            var headers = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(prop => new Header(prop)).ToArray();

            _writer.WriteLine(string.Join(",", headers.Select(header => header.Name)));

            foreach (var record in records)
            {
                _writer.WriteLine(string.Join(",", headers.Select(header =>
                {
                    var prop = header.Property.GetValue(record);
                    var val = prop?.ToString() ?? "";

                    if (val.Contains(","))
                        val = '"' + val.Replace("\\", "\\\\")
                            .Replace("\"", "\\\"") + '"';

                    return val;
                })));
            }
        }
    }
}