using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MRRC.Cursive
{
    public class CsvReader
    {
        // From https://stackabuse.com/regex-splitting-by-character-unless-in-quotes/
        private static readonly Regex CsvLineRegex = new Regex(",(?=([^\"]*\"[^\"]*\")*[^\"]*$)");
        
        private static IEnumerable<PropertyInfo> GetProperties<T>(IReadOnlyList<string> headers)
        {
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var res = new PropertyInfo[headers.Count];

            for (var i = 0; i < headers.Count; i++)
            {
                var header = headers[i];

                var property = props
                    .First(p => (p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name) == header);

                res[i] = property;
            }

            return res;
        }
        
        private readonly TextReader _reader;
        private readonly CultureInfo _culture;

        public CsvReader(TextReader reader, CultureInfo culture)
        {
            _reader = reader;
            _culture = culture;
        }

        private object GetValue(Type type, string source)
        {
            if (type == typeof(string)) return source;
            if (type == typeof(bool)) return bool.Parse(source);
            if (type == typeof(int)) return int.Parse(source);
            if (type == typeof(double)) return double.Parse(source);
            if (type == typeof(DateTime)) return DateTime.Parse(source, _culture);
            if (type.IsEnum) return Enum.Parse(type, source, true);
            throw new Exception($"Unknown type {type}");
        }

        public IEnumerable<T> GetRecords<T>()
        {
            var line = _reader.ReadLine();
            if (line == null) yield break;

            var headers = line.Split(',');
            var properties = GetProperties<T>(headers).ToArray();

            var lineNum = 0;
            while ((line = _reader.ReadLine()) != null)
            {
                var parts = CsvLineRegex.Split(line);
                
                if (parts.Length != headers.Length) 
                    throw new IndexOutOfRangeException($"Expecting {headers.Length} columns, got {parts.Length} (l{lineNum})");

                var obj = Activator.CreateInstance<T>();

                for (var i = 0; i < headers.Length; i++)
                {
                    var prop = properties[i];
                    var part = parts[i];

                    var value = GetValue(prop.PropertyType, part);
                    prop.SetValue(obj, value);
                }

                yield return obj;

                lineNum++;
            }
        }
    }
}