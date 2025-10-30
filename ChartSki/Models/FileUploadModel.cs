namespace ChartSki.Models
{
    public class FileUploadResponse
    {
        public List<string> Columns { get; set; } = new();
        public List<Dictionary<string, string>> Preview { get; set; } = new();
        public int RowsCount { get; set; }
        public List<Dictionary<string, string>>? Data { get; set; } = null;
    }
}
