namespace ChartSki.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ChartSki.Models;
    using ChartSki.Services;

    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile? file, [FromForm] string? includeFullData)
        {
            if (file == null || file.Length == 0) return BadRequest("No file is found");

            try
            {
                var result = FileParserService.ParseByFileName(file);

                var (columns, rows) = result!.Value;

                var preview = rows.Take(5).ToList();
                var response = new FileUploadResponse
                {
                    Columns = columns,
                    Preview = preview,
                    RowsCount = rows.Count
                };

                if (!string.IsNullOrEmpty(includeFullData) && includeFullData.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    response.Data = rows;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}