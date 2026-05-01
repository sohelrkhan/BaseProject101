using SadaqaAccounting.Application.ApplicationLogic.Reports.MonthlyIncomeExpenseLogic.Model;
using SadaqaAccounting.Application.ApplicationLogic.Reports.MonthlyIncomeExpenseLogic.Queries;

namespace SadaqaAccounting.Api.Controllers.Reports
{
    public class IncomeExpenseReportController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(MonthlyIncomeExpenseGridModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<MonthlyIncomeExpenseGridModel>> GetMonthlyIncomeExpenseAsync(GetMonthlyIncomeExpenseReportQuery request)
        {
            var incomeExpenseReports = await Mediator.Send(request);
            return Ok(incomeExpenseReports);
        }
        [HttpPost]
        public async Task<IActionResult> DownloadIncomeExpenseBalance(GetMonthlyIncomeExpenseReportExportQuery request)
        {
            var fileBytes = await Mediator.Send(request);

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileName;
            string contentType;

            if (request.ReportType?.ToLower() == "pdf")
            {
                fileName = $"IncomeExpense{timestamp}.pdf";
                contentType = "application/pdf";
            }
            else if (request.ReportType?.ToLower() == "excel")
            {
                fileName = $"IncomeExpense{timestamp}.xlsx";
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else
            {
                return BadRequest("Unsupported file format requested.");
            }

            var exportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExportedFiles");
            Directory.CreateDirectory(exportDirectory);
            var filePath = Path.Combine(exportDirectory, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, contentType, fileName);
        }
    }

}
