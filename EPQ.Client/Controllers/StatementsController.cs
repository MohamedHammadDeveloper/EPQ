using EPQ.Client.Conifigrations.Table.Interfaces;
using EPQ.Client.RequestDtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace EPQ.Client.Controllers
{
    [ApiController]
    [Route("api/statements")]
    public class StatementsController : ControllerBase
    {
        private readonly IAccountStatementService _svc;

        public StatementsController(IAccountStatementService svc) => _svc = svc;

        [HttpPost("load")]
        public async Task<IActionResult> Load()
        {
            // DataTables form fields
            var draw = int.TryParse(Request.Form["draw"], out var d) ? d : 1;
            var start = int.TryParse(Request.Form["start"], out var s) ? s : 0;
            var length = int.TryParse(Request.Form["length"], out var l) ? l : 10;
            var sortColIndex = Request.Form["order[0][column]"].FirstOrDefault();
            var sortColumn = Request.Form[$"columns[{sortColIndex}][name]"].FirstOrDefault();
            var sortDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var search = Request.Form["search[value]"].FirstOrDefault();

            // custom filters
            int accountId = 0;
            if (int.TryParse(Request.Form["accountId"],out accountId))
                accountId = accountId;
            DateTime dateFrom = DateTime.Parse(Request.Form["dateFrom"]);
            DateTime dateTo = DateTime.Parse(Request.Form["dateTo"]);

            var req = new StatementRequest
            {
                Draw = draw,
                Start = start,
                Length = length,
                SortColumn = sortColumn,
                SortDir = sortDir,
                Search = search,
                AccountId = Convert.ToInt32(accountId),
                DateFrom = dateFrom,
                DateTo = dateTo
            };

            var result = await _svc.GetStatementAsync(req, HttpContext.RequestAborted);

            // DataTables expects { draw, recordsTotal, recordsFiltered, data }
            return Ok(new
            {
                draw = result.Draw,
                recordsTotal = result.RecordsFiltered,
                recordsFiltered = result.RecordsFiltered,
                data = result.Data,
                totals = result.Totals   
            });
        }


        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var row = await _svc.GetTxDetailsAsync(id, HttpContext.RequestAborted);
            

            var text = Regex.Replace(row.Description.ToString(), @"^(<br\s*/?>\s*)+", "");
            text = Regex.Replace(text, @"(\s*<br\s*/?>)+$", "");
            row.Description = text;

            return row is null ? NotFound() : Ok(row);
        }

        // Excel
        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] int accountId, [FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
        {
            var req = new StatementRequest { AccountId = accountId, DateFrom = dateFrom, DateTo = dateTo, Draw = 1, Start = 0, Length = int.MaxValue };
            var data = await _svc.GetStatementAsync(req, HttpContext.RequestAborted);

            using var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.AddWorksheet("Statement");
            ws.Cell(1, 1).Value = "Account ID";
            ws.Cell(1, 2).Value = "Account Name";
            ws.Cell(1, 3).Value = "Txn Date";
            ws.Cell(1, 4).Value = "Previous";
            ws.Cell(1, 5).Value = "Debit";
            ws.Cell(1, 6).Value = "Credit";
            ws.Cell(1, 7).Value = "Final";

            int r = 2;
            foreach (var x in data.Data)
            {
                ws.Cell(r, 1).Value = x.AccountId;
                ws.Cell(r, 2).Value = x.AccountName;
                ws.Cell(r, 3).Value = x.TxnDate;
                ws.Cell(r, 4).Value = x.PreviousBalance;
                ws.Cell(r, 5).Value = x.Debit;
                ws.Cell(r, 6).Value = x.Credit;
                ws.Cell(r, 7).Value = x.FinalBalance;
                r++;
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var fileName = $"Statement_{accountId}_{dateFrom:yyyyMMdd}_{dateTo:yyyyMMdd}.xlsx";
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }

}
