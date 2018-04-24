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
        private readonly IPatientRepository _repository;

        public PatientController(IPatientRepository repository)
        {
            _repository = repository;
        }

        // GET: Patients
        public IActionResult Index()
        {
            return View(_repository.GetAll().ToList());
        }

        // GET: Patients/Details/<key>
        public IActionResult Details(string id)
        {
            var patient = _repository.Find(id);
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

        // GET: Patients/Edit/5
        public IActionResult Edit(string id)
        {
            var patient = _repository.Find(id);
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
                    if (!PatientExists(patient.PatientId))
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

        // GET: Patients/Delete/5
        public IActionResult Delete(string id)
        {
            var patient = _repository.Find(id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var patient = _repository.Find(id);
            _repository.Delete(patient);
            _repository.Save();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(string id)
        {
            return _repository.Any(e => e.PatientId == id);
        }
    }
}
