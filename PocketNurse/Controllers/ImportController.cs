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
            if (file == null) return RedirectToAction("Index");

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

                    var patients = new List<PatientDescription>();
                    var notInFormulary = new List<NotInFormulary>();

                    // There should be three worksheets
                    if (pck.Workbook.Worksheets.Count != 3) return RedirectToAction("Index");
                    // Not enough rows to include header and data
                    if (pck.Workbook.Worksheets[1].Dimension.Rows < 1) return RedirectToAction("Index");
                    if (pck.Workbook.Worksheets[1].Dimension.Rows > 1)
                    {
                        // Not enough columns to include header and data
                        if (pck.Workbook.Worksheets[1].Dimension.Columns != 6) return RedirectToAction("Index");
                        // Process data for this sheet
                        for (int i = pck.Workbook.Worksheets[1].Dimension.Start.Row + 1;
                                 i <= pck.Workbook.Worksheets[1].Dimension.End.Row;
                                 i++)
                        {
                            var patientDescription = new PatientDescription()
                            {
                                Patient = new Patient()
                            };
                            patientDescription.Patient.First = (string)pck.Workbook.Worksheets[1].Cells[i, 1].Value;
                            patientDescription.Patient.Last = (string)pck.Workbook.Worksheets[1].Cells[i, 2].Value;
                            patientDescription.Patient.FullName = patientDescription.Patient.First + " " + patientDescription.Patient.Last;
                            var re = new Regex(@"(\d{1,2})[/-](\d{1,2})");
                            if(pck.Workbook.Worksheets[1].Cells[i, 2].Value != null)
                            {
                                var m = re.Match((string)pck.Workbook.Worksheets[1].Cells[i, 3].Value);
                                if (m.Success)
                                {
                                    patientDescription.Patient.DOB = new DateTime(1, int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
                                }
                                else
                                {
                                    // Invalid date format
                                    return RedirectToAction("Index");
                                }
                            }
                            patientDescription.Patient.MRN = Convert.ToString(pck.Workbook.Worksheets[1].Cells[i, 4].Value);
                            patientDescription.Patient.PatientId = (string)pck.Workbook.Worksheets[1].Cells[i, 5].Value;
                            if(patientDescription.Patient.PatientId == null)
                            {
                                patientDescription.Patient.PatientId = patientDescription.Patient.MRN;
                            }
                            var allergiesString = (string)pck.Workbook.Worksheets[1].Cells[i, 6].Value;
                            if(allergiesString != null)
                            {
                                foreach(var allergy in allergiesString.Split(','))
                                {
                                    patientDescription.Allergies.Add(new Allergy() { AllergyId = Guid.Empty, AllergyName = allergy });
                                }
                            }
                            patients.Add(patientDescription);
                        }
                    }
                    if (pck.Workbook.Worksheets[2].Dimension.Rows < 1) return RedirectToAction("Index");
                    if (pck.Workbook.Worksheets[2].Dimension.Rows > 1)
                    {
                        // Process data for this sheet
                        for (int i = pck.Workbook.Worksheets[2].Dimension.Start.Row + 1;
                                 i <= pck.Workbook.Worksheets[2].Dimension.End.Row;
                                 i++)
                        {
                            var patientId = Convert.ToString(pck.Workbook.Worksheets[2].Cells[i, 6].Value);
                            var patientDescription = patients.First(p => p.Patient.PatientId == patientId);
                            if(patientDescription != null)
                            {
                                patientDescription.MedicationOrders.Add(new MedicationOrder()
                                {
                                    MedicationId = Guid.Empty,
                                    //PocketNurseItemId = (string)pck.Workbook.Worksheets[1].Cells[i, 1].Value,
                                    MedicationName = (string)pck.Workbook.Worksheets[2].Cells[i, 2].Value,
                                    Dose = (string)pck.Workbook.Worksheets[2].Cells[i, 3].Value,
                                    Frequency = (string)pck.Workbook.Worksheets[2].Cells[i, 4].Value,
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
                            notInFormulary.Add(new NotInFormulary()
                            {
                                _id = -1,
                                GenericName = (string)pck.Workbook.Worksheets[3].Cells[i, 1].Value,
                                Alias = (string)pck.Workbook.Worksheets[3].Cells[i, 2].Value,
                                Strength = pck.Workbook.Worksheets[3].Cells[i, 3].Value == null ? -1 : int.Parse((string)pck.Workbook.Worksheets[3].Cells[i, 3].Value),
                                StrengthUnit = (string)pck.Workbook.Worksheets[3].Cells[i, 4].Value,
                                Volume = pck.Workbook.Worksheets[3].Cells[i, 5].Value == null ? -1 : int.Parse((string)pck.Workbook.Worksheets[3].Cells[i, 5].Value),
                                VolumeUnit = (string)pck.Workbook.Worksheets[3].Cells[i, 6].Value,
                                TotalContainerVolume = (string)pck.Workbook.Worksheets[3].Cells[i, 7].Value,
                                Route = (string)pck.Workbook.Worksheets[3].Cells[i, 8].Value,
                            });
                        }
                    }

                    // process uploaded files
                    // Don't rely on or trust the FileName property without validation.

                    return Ok(new { size, wsCount, patients, notInFormulary });
                }
            }
            return NotFound();
        }
    }
}