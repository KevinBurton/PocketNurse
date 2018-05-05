using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PocketNurse.Models;
using PocketNurse.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PocketNurse.Controllers
{
    [Authorize]
    public class ImportController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPocketNurseRepository _pocketNurseRepository;

        public ImportController(UserManager<ApplicationUser> userManager, IPocketNurseRepository pocketNurseRepository)
        {
            _userManager = userManager;
            _pocketNurseRepository = pocketNurseRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // https://stackoverflow.com/questions/30701006/how-to-get-the-current-logged-in-user-id-asp-net-core
            var currentUser = User.FindFirst(ClaimTypes.Name)?.Value;
            // https://stackoverflow.com/questions/35781423/how-should-i-access-my-applicationuser-properties-from-within-my-mvc-6-views
            if (User != null && User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var appUser = await _userManager.FindByNameAsync(User.Identity.Name);
                if(appUser == null)
                {
                    // No file
                    ModelState.AddModelError("cabinet", "NULL ApplicationUser in Upload action in Import controller");
                    return RedirectToAction("Index");
                }
            }
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
                        var patientDescriptionList = cabinetView.Patients.Where(pd => medicationOrderRaw.PatientIdList.Contains(pd.Patient.PatientId)).ToList();
                        if(patientDescriptionList.Count != medicationOrderRaw.PatientIdList.Count)
                        {
                            var patientDiffList = medicationOrderRaw.PatientIdList.Except<string>(patientDescriptionList.Select(pd => pd.Patient.PatientId));
                            // Patients not found
                            ModelState.AddModelError("medication-order", $"No patients found for the medication order for '{medicationOrderRaw.MedicationName}' for patients '{string.Join(",", patientDiffList)}'");
                        }
                        foreach (var patientDescription in patientDescriptionList)
                        {
                            if(patientDescription != null)
                            {
                                patientDescription.MedicationOrders.Add(new MedicationOrder()
                                {
                                    PocketNurseItemId = medicationOrderRaw.PocketNurseItemId,
                                    MedicationName = medicationOrderRaw.MedicationName,
                                    Dose = medicationOrderRaw.Dose,
                                    Frequency = medicationOrderRaw.Frequency,
                                    Route = medicationOrderRaw.Route,
                                    Patient = patientDescription.Patient
                                });
                            }
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
            if (wk.Name != "Session" ||
                ((string)wk.Cells[1, 1].Value).Trim() != "Session" ||
                ((string)wk.Cells[1, 2].Value).Trim() != "Date" ||
                ((string)wk.Cells[1, 3].Value).Trim() != "State" ||
                ((string)wk.Cells[1, 4].Value).Trim() != "Area")
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
                        patientDescription.Allergies.Add(new Allergy() { AllergyName = allergy });
                    }
                }
                retval.Add(patientDescription);
            }
            return retval;
        }

        private List<MedicationOrderRaw> ReadMedicationOrderWorkSheet(ExcelWorksheet wk)
        {
            var retval = new List<MedicationOrderRaw>();
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
                return retval;
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