namespace ChartSki.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ChartSki.Models;
    using System.Globalization;

    [ApiController]
    [Route("api/[controller]")]
    public class ChartController : ControllerBase
    {
        [HttpPost]
        public IActionResult Visualise([FromBody] ChartRequest req)
        {
            if (req == null || req.Rows == null || !req.Rows.Any()) return BadRequest("No rows provided");
            if (string.IsNullOrEmpty(req.LabelColumn)) return BadRequest("labelColumn required");
            if (req.ValueColumns == null || !req.ValueColumns.Any()) return BadRequest("valueColumns required");

            var chart = new ChartData();

            chart.Labels = req.Rows.Select(r => r.ContainsKey(req.LabelColumn) ? r[req.LabelColumn] ?? string.Empty : string.Empty).ToList();

            foreach (var valCol in req.ValueColumns)
            {
                var ds = new ChartDataset { Label = valCol, Data = new List<double>() };
                foreach (var row in req.Rows)
                {
                    if (!row.TryGetValue(valCol, out var sval) || string.IsNullOrWhiteSpace(sval))
                    {
                        ds.Data.Add(double.NaN);
                        continue;
                    }

                    // Number parsing (commas, dollar sign)
                    var cleaned = sval.Replace(",", "").Replace("$", "").Trim();
                    if (double.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                    {
                        ds.Data.Add(d);
                    }
                    else
                    {
                        ds.Data.Add(double.NaN);
                    }
                }
                chart.Datasets.Add(ds);
            }

            return Ok(chart);
        }
    }
}
