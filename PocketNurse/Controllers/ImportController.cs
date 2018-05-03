using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
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
                // process uploaded file
                // Don't rely on or trust the FileName property without validation.
                using (var pck = new OfficeOpenXml.ExcelPackage())
                {
                    using (var stream = file.OpenReadStream())
                    {
                        pck.Load(stream);
                    }

                    var wsCount = pck.Workbook.Worksheets.Count();

                    // There should be four worksheets
                    if (wsCount < 4)
                    {
                        // Not enough worksheets
                        ModelState.AddModelError("cabinet", $"Not enough worksheets -> {pck.Workbook.Worksheets.Count}");
                        return RedirectToAction("Index");
                    }
                    OmnicellCabinetViewModel cabinetView = ReadSessionWorkSheet(pck.Workbook.Worksheets[0]);
                    if(cabinetView == null)
                    {
                        return RedirectToAction("Index");
                    }

                    cabinetView.Patients.AddRange(ReadPatientWorkSheet(pck.Workbook.Worksheets[1]));

                    var medicationOrderRawList = ReadMedicationOrderWorkSheet(pck.Workbook.Worksheets[2]);

                    foreach(var medicationOrderRaw in medicationOrderRawList)
                    {
                        var found = false;
                        foreach(var patientId in medicationOrderRaw.PatientIdList)
                        {
                            var patientDescription = cabinetView.Patients.FirstOrDefault(p => p.Patient.PatientId == patientId);
                            if(patientDescription != null)
                            {
                                found = true;
                                patientDescription.MedicationOrders.Add(new MedicationOrder()
                                {
                                    MedicationId = Guid.Empty,
                                    PocketNurseItemId = medicationOrderRaw.PocketNurseItemId,
                                    MedicationName = medicationOrderRaw.MedicationName,
                                    Dose = medicationOrderRaw.Dose,
                                    Frequency = medicationOrderRaw.Frequency,
                                    Route = medicationOrderRaw.Route,
                                    Patient = patientDescription.Patient
                                });
                            }
                        }
                        if(!found)
                        {
                            // Patient not found
                            ModelState.AddModelError("medication-order", $"No patient found for the medication order for '{medicationOrderRaw.MedicationName}' for patients '{string.Join(",", medicationOrderRaw.PatientIdList)}'");
                        }
                    }

                    cabinetView.NotInFormulary.AddRange(ReadNotInFormularyWorkSheet(pck.Workbook.Worksheets[3]));

                    return View(cabinetView);
                }
            }
            return NotFound();
        }
        #region Helpers
        private OmnicellCabinetViewModel ReadSessionWorkSheet(ExcelWorksheet wk)
        {
            if (wk.Dimension.Rows <= 1)
            {
                // Not enough rows
                ModelState.AddModelError("session", $"Session worksheet doesn't have enough rows {wk.Dimension.Rows}");
                return null;
            }
            if (wk.Dimension.Columns < 4)
            {
                // Not enough columns
                ModelState.AddModelError("session", $"Session worksheet doesn't have enough columns {wk.Dimension.Columns}");
                return null;
            }
            // Skip blank rows
            if (wk.Cells[2, 1].Value == null ||
                wk.Cells[2, 2].Value == null ||
                wk.Cells[2, 3].Value == null ||
                wk.Cells[2, 4].Value == null)
            {
                // Invalid content
                ModelState.AddModelError("session", "Session worksheet has invlid content");
                return null;
            }
            var cabinetSessionId = Convert.ToString(wk.Cells[2, 1].Value);
            var dateNum = 0.0;
            if (wk.Cells[2, 2].Value is double)
            {
                dateNum = (double)wk.Cells[2, 2].Value;
            }
            else
            {
                dateNum = double.Parse(Convert.ToString(wk.Cells[2, 2].Value));
            }
            var cabinetSessionDate = DateTime.FromOADate(dateNum);
            var cabinetState = Convert.ToString(wk.Cells[2, 3].Value);
            var cabinetArea = Convert.ToString(wk.Cells[2, 4].Value);
            var cabinet = new Cabinet() { State = cabinetState, Area = cabinetArea };
            var cabinetSession = new CabinetSession() { CabinetSessionId = cabinetSessionId, Cabinet = cabinet, TimeStamp = cabinetSessionDate };

            return new OmnicellCabinetViewModel(cabinetSession);
        }

        private List<PatientDescription> ReadPatientWorkSheet(ExcelWorksheet wk)
        {
            var retval = new List<PatientDescription>();
            // Not enough rows to include header and data for patient
            if (wk.Dimension.Rows <= 1)
            {
                // Not enough rows
                ModelState.AddModelError("patient", $"Patient worksheet doesn't have enough rows {wk.Dimension.Rows}");
                return retval;
            }
            if (wk.Dimension.Columns < 6)
            {
                // Not enough columns
                ModelState.AddModelError("patient", $"Patient worksheet doesn't have enough columns {wk.Dimension.Columns}");
                return retval;
            }
            // Process data for this sheet
            // Skip the headers
            for (int i = wk.Dimension.Start.Row + 1;
                     i <= wk.Dimension.End.Row;
                     i++)
            {
                // Skip blank rows
                if (wk.Cells[i, 1].Value == null ||
                    wk.Cells[i, 2].Value == null) continue;

                var patientDescription = new PatientDescription()
                {
                    Patient = new Patient()
                };
                patientDescription.Patient.First = (string)wk.Cells[i, 1].Value;
                patientDescription.Patient.Last = (string)wk.Cells[i, 2].Value;
                patientDescription.Patient.FullName = patientDescription.Patient.First + " " + patientDescription.Patient.Last;
                if (wk.Cells[i, 3].Value != null)
                {
                    if (wk.Cells[i, 3].Value.GetType() == typeof(DateTime))
                    {
                        patientDescription.Patient.DOB = (DateTime)wk.Cells[i, 3].Value;
                    }
                    else if (wk.Cells[i, 3].Value.GetType() == typeof(string))
                    {
                        var re = new Regex(@"(\d{1,2})[/-](\d{1,2})");
                        var m = re.Match((string)wk.Cells[i, 3].Value);
                        if (m.Success)
                        {
                            patientDescription.Patient.DOB = new DateTime(1, int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
                        }
                        else
                        {
                            // Invalid date format
                            ModelState.AddModelError("patient", $"Patient DOB is a string but it is an urecognizable format {(string)wk.Cells[i, 3].Value}");
                        }
                    }
                    else
                    {
                        // Invalid type
                        ModelState.AddModelError("patient", $"Patient DOB is not a string and not DateTime {wk.Cells[i, 3].Value.GetType()}");
                    }
                }
                patientDescription.Patient.MRN = Convert.ToString(wk.Cells[i, 4].Value);
                patientDescription.Patient.PatientId = (string)wk.Cells[i, 5].Value;
                if (patientDescription.Patient.PatientId == null)
                {
                    patientDescription.Patient.PatientId = patientDescription.Patient.MRN;
                }
                var allergiesString = (string)wk.Cells[i, 6].Value;
                if (allergiesString != null)
                {
                    foreach (var allergy in allergiesString.Split(','))
                    {
                        patientDescription.Allergies.Add(new Allergy() { AllergyId = Guid.Empty, AllergyName = allergy });
                    }
                }
                retval.Add(patientDescription);
            }
            return retval;
        }

        private List<MedicationOrderRaw> ReadMedicationOrderWorkSheet(ExcelWorksheet wk)
        {
            var retval = new List<MedicationOrderRaw>();
            if (wk.Dimension.Rows <= 1)
            {
                // Not enough rows (no medication orders)
                ModelState.AddModelError("medication-order", $"No medication orders {wk.Dimension.Rows}");
            }
            else
            {
                // Process data for this sheet
                // Skip the headers
                for (int i = wk.Dimension.Start.Row + 1;
                         i <= wk.Dimension.End.Row;
                         i++)
                {
                    // Order not associated with a patient?
                    if (wk.Cells[i, 6].Value == null) continue;

                    var medicationOrder = new MedicationOrderRaw();
                    var patientIdIndex = 6;
                    do
                    {
                        var patientId = Convert.ToString(wk.Cells[i, patientIdIndex].Value);
                        medicationOrder.PatientIdList.Add(patientId);
                    } while (patientIdIndex > wk.Dimension.End.Column ||
                             wk.Cells[i, patientIdIndex].Value == null);
                    medicationOrder.PocketNurseItemId = Convert.ToString(wk.Cells[i, 1].Value);
                    medicationOrder.MedicationName = (string)wk.Cells[i, 2].Value;
                    medicationOrder.Dose = Convert.ToString(wk.Cells[i, 3].Value);
                    medicationOrder.Frequency = Convert.ToString(wk.Cells[i, 4].Value);
                    medicationOrder.Route = Convert.ToString(wk.Cells[i, 5].Value);
                    retval.Add(medicationOrder);
                }
            }
            return retval;
        }

        private List<NotInFormulary> ReadNotInFormularyWorkSheet(ExcelWorksheet wk)
        {
            var retval = new List<NotInFormulary>();
            if (wk.Dimension.Rows <= 1)
            {
                // Not enough rows (not in formulary)
                ModelState.AddModelError("notinformulary", $"No NotInFormulary rows {wk.Dimension.Rows}");
            }
            else if (wk.Dimension.Columns < 8)
            {
                // Not enough columns (not in formulary)
                ModelState.AddModelError("notinformulary", $"Not enough NotInFormulary columns {wk.Dimension.Columns}");
            }
            else
            {
                // Process data for this sheet
                // Skip the headers
                for (int i = wk.Dimension.Start.Row + 1;
                         i <= wk.Dimension.End.Row;
                         i++)
                {
                    retval.Add(new NotInFormulary()
                    {
                        _id = -1,
                        GenericName = (string)wk.Cells[i, 1].Value,
                        Alias = (string)wk.Cells[i, 2].Value,
                        Strength = wk.Cells[i, 3].Value == null ? string.Empty : wk.Cells[i, 3].Value.GetType() == typeof(string) ? (string)wk.Cells[i, 3].Value : Convert.ToString(wk.Cells[i, 3].Value),
                        StrengthUnit = (string)wk.Cells[i, 4].Value,
                        Volume = wk.Cells[i, 5].Value == null ? string.Empty : wk.Cells[i, 5].Value.GetType() == typeof(string) ? (string)wk.Cells[i, 5].Value : Convert.ToString(wk.Cells[i, 5].Value),
                        VolumeUnit = (string)wk.Cells[i, 6].Value,
                        TotalContainerVolume = (string)wk.Cells[i, 7].Value,
                        Route = (string)wk.Cells[i, 8].Value,
                    });
                }
            }
            return retval;
        }
        #endregion
    }
}