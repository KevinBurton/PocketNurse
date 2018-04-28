using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PocketNurse.Models;
using PocketNurse.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PocketNurse.Controllers
{
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
            // Bail out of the file was not uploaded
            if (file == null)
            {
                // No file
                ModelState.AddModelError("cabinet", "NULL file passed to Upload action in Import controller");
                return RedirectToAction("Index");
            }

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

                    var cabinet = new OmnicellCabinetViewModel();

                    // There should be three worksheets
                    if (pck.Workbook.Worksheets.Count < 3)
                    {
                        // Not enough worksheets
                        ModelState.AddModelError("cabinet", $"Not enough worksheets -> {pck.Workbook.Worksheets.Count}");
                        return RedirectToAction("Index");
                    }
                    // Not enough rows to include header and data
                    if (pck.Workbook.Worksheets[1].Dimension.Rows <= 1)
                    {
                        // Not enough rows
                        ModelState.AddModelError("patient", $"Patient worksheet doesn't have enough rows {pck.Workbook.Worksheets[1].Dimension.Rows}");
                    }
                    if (pck.Workbook.Worksheets[1].Dimension.Rows > 1)
                    {
                        // Not enough columns to include header and data
                        if (pck.Workbook.Worksheets[1].Dimension.Columns < 6)
                        {
                            // Not enough columns
                            ModelState.AddModelError("patient", $"Patient worksheet doesn't have enough columns {pck.Workbook.Worksheets[1].Dimension.Columns}");
                        }
                        else
                        {
                            // Process data for this sheet
                            for (int i = pck.Workbook.Worksheets[1].Dimension.Start.Row + 1;
                                     i <= pck.Workbook.Worksheets[1].Dimension.End.Row;
                                     i++)
                            {
                                // Skip blank rows
                                if (pck.Workbook.Worksheets[1].Cells[i, 1].Value == null ||
                                    pck.Workbook.Worksheets[1].Cells[i, 2].Value == null) continue;

                                var patientDescription = new PatientDescription()
                                {
                                    Patient = new Patient()
                                };
                                patientDescription.Patient.First = (string)pck.Workbook.Worksheets[1].Cells[i, 1].Value;
                                patientDescription.Patient.Last = (string)pck.Workbook.Worksheets[1].Cells[i, 2].Value;
                                patientDescription.Patient.FullName = patientDescription.Patient.First + " " + patientDescription.Patient.Last;
                                if (pck.Workbook.Worksheets[1].Cells[i, 3].Value != null)
                                {
                                    if (pck.Workbook.Worksheets[1].Cells[i, 3].Value.GetType() == typeof(DateTime))
                                    {
                                        patientDescription.Patient.DOB = (DateTime)pck.Workbook.Worksheets[1].Cells[i, 3].Value;
                                    }
                                    else if (pck.Workbook.Worksheets[1].Cells[i, 3].Value.GetType() == typeof(string))
                                    {
                                        var re = new Regex(@"(\d{1,2})[/-](\d{1,2})");
                                        var m = re.Match((string)pck.Workbook.Worksheets[1].Cells[i, 3].Value);
                                        if (m.Success)
                                        {
                                            patientDescription.Patient.DOB = new DateTime(1, int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
                                        }
                                        else
                                        {
                                            // Invalid date format
                                            ModelState.AddModelError("patient", $"Patient DOB is a string but it is an urecognizable format {(string)pck.Workbook.Worksheets[1].Cells[i, 3].Value}");
                                        }
                                    }
                                    else
                                    {
                                        // Invalid type
                                        ModelState.AddModelError("patient", $"Patient DOB is not a string and not DateTime {pck.Workbook.Worksheets[1].Cells[i, 3].Value.GetType()}");
                                    }
                                }
                                patientDescription.Patient.MRN = Convert.ToString(pck.Workbook.Worksheets[1].Cells[i, 4].Value);
                                patientDescription.Patient.PatientId = (string)pck.Workbook.Worksheets[1].Cells[i, 5].Value;
                                if (patientDescription.Patient.PatientId == null)
                                {
                                    patientDescription.Patient.PatientId = patientDescription.Patient.MRN;
                                }
                                var allergiesString = (string)pck.Workbook.Worksheets[1].Cells[i, 6].Value;
                                if (allergiesString != null)
                                {
                                    foreach (var allergy in allergiesString.Split(','))
                                    {
                                        patientDescription.Allergies.Add(new Allergy() { AllergyId = Guid.Empty, AllergyName = allergy });
                                    }
                                }
                                cabinet.Patients.Add(patientDescription);
                            }
                        }
                    }
                    if (pck.Workbook.Worksheets[2].Dimension.Rows <= 1)
                    {
                        // Not enough rows (no medication orders)
                        ModelState.AddModelError("medication-order", $"No medication orders {pck.Workbook.Worksheets[2].Dimension.Rows}");
                    }
                    if (pck.Workbook.Worksheets[2].Dimension.Rows > 1)
                    {
                        // Process data for this sheet
                        for (int i = pck.Workbook.Worksheets[2].Dimension.Start.Row + 1;
                                 i <= pck.Workbook.Worksheets[2].Dimension.End.Row;
                                 i++)
                        {
                            if (pck.Workbook.Worksheets[2].Cells[i, 6].Value == null) continue;
                            PatientDescription patientDescription = null;
                            var patientIdList = new List<string>();
                            var patientIdIndex = 6;
                            do
                            {
                                if (patientIdIndex > pck.Workbook.Worksheets[2].Dimension.End.Column ||
                                    pck.Workbook.Worksheets[2].Cells[i, patientIdIndex].Value == null) break;
                                var patientId = Convert.ToString(pck.Workbook.Worksheets[2].Cells[i, patientIdIndex].Value);
                                patientIdList.Add(patientId);
                                patientDescription = cabinet.Patients.FirstOrDefault(p => p.Patient.PatientId == patientId);
                                patientIdIndex++;
                            } while (patientDescription == null);
                            if(patientDescription == null)
                            {
                                // Patient not found
                                ModelState.AddModelError("medication-order", $"No patient found for the medication order on line {i} {string.Join(",",patientIdList)}");
                                continue;
                            }
                            else
                            {
                                patientDescription.MedicationOrders.Add(new MedicationOrder()
                                {
                                    MedicationId = Guid.Empty,
                                    PocketNurseItemId = (string)pck.Workbook.Worksheets[2].Cells[i, 1].Value,
                                    MedicationName = (string)pck.Workbook.Worksheets[2].Cells[i, 2].Value,
                                    Dose = Convert.ToString(pck.Workbook.Worksheets[2].Cells[i, 3].Value),
                                    Frequency = Convert.ToString(pck.Workbook.Worksheets[2].Cells[i, 4].Value),
                                    Route = (string)pck.Workbook.Worksheets[2].Cells[i, 5].Value,
                                    Patient = patientDescription.Patient
                                });
                            }
                        }
                    }
                    if (pck.Workbook.Worksheets[3].Dimension.Rows < 1) return RedirectToAction("Index");
                    if (pck.Workbook.Worksheets[3].Dimension.Rows > 1)
                    {
                        // Process data for this sheet
                        for (int i = pck.Workbook.Worksheets[3].Dimension.Start.Row + 1;
                                 i <= pck.Workbook.Worksheets[3].Dimension.End.Row;
                                 i++)
                        {
                            cabinet.NotInFormulary.Add(new NotInFormulary()
                            {
                                _id = -1,
                                GenericName = (string)pck.Workbook.Worksheets[3].Cells[i, 1].Value,
                                Alias = (string)pck.Workbook.Worksheets[3].Cells[i, 2].Value,
                                Strength = pck.Workbook.Worksheets[3].Cells[i, 3].Value == null ? string.Empty : pck.Workbook.Worksheets[3].Cells[i, 3].Value.GetType() == typeof(string) ? (string)pck.Workbook.Worksheets[3].Cells[i, 3].Value : Convert.ToString(pck.Workbook.Worksheets[3].Cells[i, 3].Value),
                                StrengthUnit = (string)pck.Workbook.Worksheets[3].Cells[i, 4].Value,
                                Volume = pck.Workbook.Worksheets[3].Cells[i, 5].Value == null ? string.Empty : pck.Workbook.Worksheets[3].Cells[i, 5].Value.GetType() == typeof(string) ? (string)pck.Workbook.Worksheets[3].Cells[i, 5].Value : Convert.ToString(pck.Workbook.Worksheets[3].Cells[i, 5].Value),
                                VolumeUnit = (string)pck.Workbook.Worksheets[3].Cells[i, 6].Value,
                                TotalContainerVolume = (string)pck.Workbook.Worksheets[3].Cells[i, 7].Value,
                                Route = (string)pck.Workbook.Worksheets[3].Cells[i, 8].Value,
                            });
                        }
                    }

                    // process uploaded files
                    // Don't rely on or trust the FileName property without validation.

                    return View(cabinet);
                }
            }
            return NotFound();
        }
    }
}