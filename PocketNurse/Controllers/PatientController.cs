using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PocketNurse.Models;
using PocketNurse.Repository;

namespace PocketNurse.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPocketNurseRepository _repository;

        public PatientController(IPocketNurseRepository repository)
        {
            _repository = repository;
        }

        // GET: Patients
        public IActionResult Index()
        {
            return View(_repository.GetAllPatients().ToList());
        }

        // GET: Patients/Details/<key>/<key>
        public IActionResult Details(string patientId, int cabinetId)
        {
            var patient = _repository.FindPatient(patientId, cabinetId);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("MRN,First,Last,DOB")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _repository.Add(patient);
                _repository.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Edit/5/1
        public IActionResult Edit(string patientId, int cabinetId)
        {
            var patient = _repository.FindPatient(patientId, cabinetId);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("MRN,First,Last,DOB,")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(patient);
                    _repository.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId, patient.CabinetId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Delete/5/1
        public IActionResult Delete(string patientId, int cabinetId)
        {
            var patient = _repository.FindPatient(patientId, cabinetId);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string patientId, int cabinetId)
        {
            var patient = await _repository.FindPatient(patientId, cabinetId);
            _repository.Delete(patient);
            await _repository.Save();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(string patientId, int cabinetId)
        {
            return _repository.Any(e => e.PatientId == patientId && e.CabinetId == cabinetId);
        }
    }
}
