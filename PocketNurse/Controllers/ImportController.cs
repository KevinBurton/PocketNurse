using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PocketNurse.Repository;
using System.Collections.Generic;
using System.Linq;

namespace PocketNurse.Controllers
{
    public class WorksheetMeta
    {
        public WorksheetMeta()
        {
            rows = new List<List<object>>();
        }
        public string name { get; set; }
        public int startRow { get; set; }
        public int endRow { get; set; }
        public int startColumn { get; set; }
        public int endColumn { get; set; }
        public List<List<object>> rows { get; set; }
    }
    public class ImportController : Controller
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientAllergyRepository _patientAllergyRepository;
        private readonly IMedicationOrderRepository _medicationOrderRepository;
        private readonly INotInFormularyRepository _notInFormularyRepository;

        public ImportController(IPatientRepository patientRepository,
                                IPatientAllergyRepository patientAllergyRepository,
                                IMedicationOrderRepository medicationOrderRepository,
                                INotInFormularyRepository notInFormularyRepository)
        {
            _patientRepository = patientRepository;
            _patientAllergyRepository = patientAllergyRepository;
            _medicationOrderRepository = medicationOrderRepository;
            _notInFormularyRepository = notInFormularyRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("Upload")]
        public IActionResult Upload(IFormFile file)
        {
            long size = file.Length;

            if (file.Length > 0)
            {
                using (var pck = new OfficeOpenXml.ExcelPackage())
                {
                    using (var stream = file.OpenReadStream())
                    {
                        pck.Load(stream);
                    }

                    var wsCount = pck.Workbook.Worksheets.Count();
                    var worksheets = new List<WorksheetMeta>();
                    foreach (var ws in pck.Workbook.Worksheets)
                    {
                        var rows = new List<List<object>>();
                        for (int i = ws.Dimension.Start.Row;
                                 i <= ws.Dimension.End.Row;
                                 i++)
                        {
                            var cells = new List<object>();
                            for (int j = ws.Dimension.Start.Column;
                                     j <= ws.Dimension.End.Column;
                                     j++)
                            {
                                cells.Add(ws.Cells[i, j].Value);
                            }
                            rows.Add(cells);
                        }
                        worksheets.Add(new WorksheetMeta()
                        {
                            name = ws.Name,
                            startRow = ws.Dimension.Start.Row,
                            endRow = ws.Dimension.End.Row,
                            startColumn = ws.Dimension.Start.Column,
                            endColumn = ws.Dimension.End.Column,
                            rows = rows
                        });
                    }


                    // process uploaded files
                    // Don't rely on or trust the FileName property without validation.

                    return Ok(new { size, wsCount, worksheets });
                }
            }
            return NotFound();
        }
    }
}