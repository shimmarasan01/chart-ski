namespace ChartSki.Models
{
    public class ChartRequest
    {
        public List<string>? Columns { get; set; }
        public List<Dictionary<string, string>> Rows { get; set; } = new();
        public string LabelColumn { get; set; } = string.Empty;
        public List<string> ValueColumns { get; set; } = new();
    }

    public class ChartData
    {
        public List<string> Labels { get; set; } = new();
        public List<ChartDataset> Datasets { get; set; } = new();
    }

    public class ChartDataset
    {
        public string Label { get; set; } = string.Empty;
        public List<double> Data { get; set; } = new();
    }
}
