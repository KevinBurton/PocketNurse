using PocketNurse.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PocketNurse.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private PocketNurseContext Context;

        public PatientRepository(PocketNurseContext ctx)
        {
            Context = ctx;
        }

        public IEnumerable<Patient> GetAll()
        {
            return Context.Patient;
        }
        public void Add(Patient entity)
        {
            Context.Patient.Add(entity);
        }
        public Patient Find(string patientId)
        {
            return Context.Patient.Find(patientId);
        }
        public void Delete(Patient entity)
        {
            Context.Patient.Remove(entity);
        }
        public void DeleteAll(IEnumerable<Patient> entity)
        {
            Context.Patient.RemoveRange(entity);
        }
        public void Update(Patient entity)
        {
            var entry = Context.Patient.Where(s => s.PatientId == entity.PatientId);
            Context.Patient.Attach(entity);
        }
        public bool Any(Func<Patient, bool> predicate = null)
        {
            if(predicate == null)
                return Context.Patient.Any();
            else
                return Context.Patient.Any(predicate);
        }
        public void Save()
        {
            Context.SaveChanges();
        }
    }
}
