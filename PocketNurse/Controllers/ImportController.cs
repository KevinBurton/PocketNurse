using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PocketNurse.Models;
using PocketNurse.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PocketNurse.Controllers
{
    [Authorize]
    public class ImportController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IPocketNurseRepository _pocketNurseRepository;

        public ImportController(IUserRepository userRepository, IPocketNurseRepository pocketNurseRepository)
        {
            _userRepository = userRepository;
            _pocketNurseRepository = pocketNurseRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("Upload")]
        [ValidateAntiForgeryToken]
        public IActionResult Upload(IFormFile file)
        {
            // Bail out of the file was not uploaded
            if (file == null)
            {
                // No file
                ModelState.AddModelError("cabinet", "NULL file passed to Upload action in Import controller");
                return RedirectToAction("Index");
            }
            if (Path.GetExtension(file.FileName) != ".xlsx")
            {
                // Invalid file type (extension)
                ModelState.AddModelError("cabinet", "Invalid file extension to Upload action in Import controller");
                return RedirectToAction("Index");
            }

            long size = file.Length;

            if (file.Length > 0)
            {
                //var appUser = await GetCurrentUserAsync();
                var appUser = _userRepository.CurrentUser();

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
                    OmniSession session = ReadSessionWorkSheet(pck.Workbook.Worksheets[0]);
                    if(session == null)
                    {
                        return RedirectToAction("Index");
                    }
                    var viewModel = new TokenStringViewModel(session);

                    var patientList = ReadPatientWorkSheet(viewModel, pck.Workbook.Worksheets[1]);

                    var medicationOrderList = ReadMedicationOrderWorkSheet(viewModel, pck.Workbook.Worksheets[2]);
 
                    var notInFormularyList = ReadNotInFormularyWorkSheet(viewModel, pck.Workbook.Worksheets[3]);

                    viewModel.WriteToFiles();

                    return View(viewModel);
                }
            }
            return NotFound();
        }
        #region Helpers
        private OmniSession ReadSessionWorkSheet(ExcelWorksheet wk)
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
            if (wk.Name != "Session" ||
                ((string)wk.Cells[1, 1].Value).Trim() != "From" ||
                ((string)wk.Cells[1, 2].Value).Trim() != "To" ||
                ((string)wk.Cells[1, 3].Value).Trim() != "Site ID" ||
                ((string)wk.Cells[1, 4].Value).Trim() != "Omni ID")
            {
                // Not enough columns
                ModelState.AddModelError("session", "This doesn't appear to be a session worksheet");
                return null;
            }
            // Skip blank rows
            if (wk.Cells[2, 1].Value == null ||
                wk.Cells[2, 2].Value == null ||
                wk.Cells[2, 3].Value == null ||
                wk.Cells[2, 4].Value == null)
            {
                // Invalid content
                ModelState.AddModelError("session", "Session worksheet has invalid content");
                return null;
            }
            var cabinetFrom = Convert.ToString(wk.Cells[2, 1].Value);
            var cabinetTo = Convert.ToString(wk.Cells[2, 2].Value);
            var cabinetSiteId = Convert.ToString(wk.Cells[2, 3].Value);
            var cabinetOmniId = Convert.ToString(wk.Cells[2, 4].Value);
            return new OmniSession() { From = cabinetFrom, To = cabinetTo, SiteId = cabinetSiteId, OmniId = cabinetOmniId };
        }

        private List<string> ReadPatientWorkSheet(TokenStringViewModel viewModel, ExcelWorksheet wk)
        {
            // Not enough rows to include header and data for patient
            if (wk.Dimension.Rows <= 1)
            {
                // Not enough rows
                ModelState.AddModelError("patient", $"Patient worksheet doesn't have enough rows {wk.Dimension.Rows}");
                return new List<string>();
            }
            if (wk.Dimension.Columns < 6)
            {
                // Not enough columns
                ModelState.AddModelError("patient", $"Patient worksheet doesn't have enough columns {wk.Dimension.Columns}");
                return new List<string>();
            }
            if(wk.Name != "Patient Information" ||
               ((string)wk.Cells[1, 1].Value).Trim() != "Patient First Name" ||
               ((string)wk.Cells[1, 2].Value).Trim() != "Patient Last name" ||
               ((string)wk.Cells[1, 3].Value).Trim() != "DOB" ||
               ((string)wk.Cells[1, 4].Value).Trim() != "MRN" ||
               ((string)wk.Cells[1, 5].Value).Trim() != "Patient ID (if different than MRN)" ||
               ((string)wk.Cells[1, 6].Value).Trim() != "Allergies")
            {
                // Identification
                ModelState.AddModelError("patient", "This doesn't appear to be a patient worksheet");
                return new List<string>();
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

                var patient = new Patient
                {
                    First = (string)wk.Cells[i, 1].Value,
                    Last = (string)wk.Cells[i, 2].Value
                };
                patient.FullName = patient.First + " " + patient.Last;
                if (wk.Cells[i, 3].Value != null)
                {
                    if (wk.Cells[i, 3].Value.GetType() == typeof(DateTime))
                    {
                        patient.DOB = (DateTime)wk.Cells[i, 3].Value;
                    }
                    else if (wk.Cells[i, 3].Value.GetType() == typeof(string))
                    {
                        var re = new Regex(@"(\d{1,2})[/-](\d{1,2})");
                        var m = re.Match((string)wk.Cells[i, 3].Value);
                        if (m.Success)
                        {
                            patient.DOB = new DateTime(1, int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
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
                patient.MRN = Convert.ToString(wk.Cells[i, 4].Value);
                patient.PatientId = (string)wk.Cells[i, 5].Value;
                if (patient.PatientId == null)
                {
                    patient.PatientId = patient.MRN;
                }
                var allergies = (string)wk.Cells[i, 6].Value;
                viewModel.AddPatient(patient, allergies);
            }
            return viewModel.Patients;
        }

        private List<string> ReadMedicationOrderWorkSheet(TokenStringViewModel viewModel, ExcelWorksheet wk)
        {
            if (wk.Name != "Med Order Information" ||
                ((string)wk.Cells[1, 1].Value).Trim() != "Medication ID # (Pocket Nurse item ID)" ||
                ((string)wk.Cells[1, 2].Value).Trim() != "Medication Name" ||
                ((string)wk.Cells[1, 3].Value).Trim() != "Dose" ||
                ((string)wk.Cells[1, 4].Value).Trim() != "Frequency" ||
                ((string)wk.Cells[1, 5].Value).Trim() != "Route" ||
                (((string)wk.Cells[1, 6].Value).Trim() != "Patient ID" && ((string)wk.Cells[1, 6].Value).Trim() != "Patient ID 1"))
            {
                // Identification
                ModelState.AddModelError("medication-order", "This doesn't appear to be a medication-order worksheet");
                return new List<string>();
            }
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
                    viewModel.AddMedicationOrder(medicationOrder);
                }
            }
            return viewModel.MedicationOrders;
        }

        private List<string> ReadNotInFormularyWorkSheet(TokenStringViewModel viewModel, ExcelWorksheet wk)
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
            else if (wk.Name != "Items not in PN formulary" ||
                     ((string)wk.Cells[1, 1].Value).Trim() != "Generic Name" ||
                     ((string)wk.Cells[1, 2].Value).Trim() != "Alias" ||
                     ((string)wk.Cells[1, 3].Value).Trim() != "Strength" ||
                     ((string)wk.Cells[1, 4].Value).Trim() != "Unit" ||
                     ((string)wk.Cells[1, 5].Value).Trim() != "volume" ||
                     ((string)wk.Cells[1, 6].Value).Trim() != "unit" ||
                     (((string)wk.Cells[1, 7].Value).Trim() != "Total container volume for Liq/Inj" && ((string)wk.Cells[1, 7].Value).Trim() != "Total container volume for Liq/IV") ||
                     ((string)wk.Cells[1, 8].Value).Trim() != "Route")
            {
                // Identification
                ModelState.AddModelError("notinformulary", "This doesn't appear to be a notinformulary worksheet");
            }
            else
            {
                // Process data for this sheet
                // Skip the headers
                for (int i = wk.Dimension.Start.Row + 1;
                         i <= wk.Dimension.End.Row;
                         i++)
                {
                    viewModel.AddNotInFormulary(new NotInFormulary()
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
            return viewModel.NotInFormulary;
        }
        #endregion
    }
}