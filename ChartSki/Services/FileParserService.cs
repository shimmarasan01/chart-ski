namespace ChartSki.Services
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using System.Globalization;
    using System.Text.Json;
    public static class FileParserService
    {

        public static (List<string> columns, List<Dictionary<string, string>> rows) ParseCsv(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
                MissingFieldFound = null,
                HeaderValidated = null,
                IgnoreBlankLines = true,
            };

            using var csv = new CsvReader(reader, config);

            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord?.ToList() ?? new List<string>();

            var rows = new List<Dictionary<string, string>>();
            while (csv.Read())
            {
                var dict = new Dictionary<string, string>();
                foreach (var header in headers)
                {
                    dict[header] = csv.GetField(header) ?? string.Empty;
                }
                rows.Add(dict);
            }

            return (headers, rows);
        }

        public static (List<string> columns, List<Dictionary<string, string>> rows) ParseJson(Stream stream)
        {
            using var doc = JsonDocument.Parse(stream);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new InvalidOperationException("JSON root must be an array of objects");

            var rows = new List<Dictionary<string, string>>();
            var columnsSet = new HashSet<string>();

            foreach (var elem in doc.RootElement.EnumerateArray())
            {
                if (elem.ValueKind != JsonValueKind.Object) continue;
                var dict = new Dictionary<string, string>();
                foreach (var prop in elem.EnumerateObject())
                {
                    columnsSet.Add(prop.Name);
                    dict[prop.Name] = prop.Value.ValueKind == JsonValueKind.String
                        ? prop.Value.GetString() ?? string.Empty
                        : prop.Value.ToString() ?? string.Empty;
                }
                rows.Add(dict);
            }

            var columns = columnsSet.ToList();
            return (columns, rows);
        }

        public static (List<string> columns, List<Dictionary<string, string>> rows)? ParseByFileName(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            if (file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return ParseCsv(stream);
            }
            else if (file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                return ParseJson(stream);
            }
            else
            {
                return null;
            }
        }
    }
}
