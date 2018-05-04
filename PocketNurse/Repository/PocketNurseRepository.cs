using PocketNurse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketNurse.Repository
{
    public class PocketNurseRepository : IPocketNurseRepository
    {
        private PocketNurseContext Context;

        public PocketNurseRepository(PocketNurseContext ctx)
        {
            Context = ctx;
        }
        public IEnumerable<Patient> GetAllPatients()
        {
            return Context.Patient;
        }
        public void Add(Patient entity)
        {
            Context.Patient.Add(entity);
        }
        public async Task<Patient> FindPatient(string patientId, int cabinetId)
        {
            return await Context.Patient.FindAsync(patientId, cabinetId);
        }
        public IEnumerable<Patient> Find(Func<Patient, bool> predicate)
        {
            return Context.Patient.Where(predicate);
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
        public bool AnyPatient(Func<Patient, bool> predicate = null)
        {
            if (predicate == null)
                return Context.Patient.Any();
            else
                return Context.Patient.Any(predicate);
        }
        public IEnumerable<Cabinet> GetAllCabinets()
        {
            return Context.Cabinet;
        }
        public void Add(Cabinet entity)
        {
            Context.Cabinet.Add(entity);
        }
        public async Task<Cabinet> FindCabinet(int cabinetId)
        {
            return await Context.Cabinet.FindAsync(cabinetId);
        }
        public IEnumerable<Cabinet> Find(Func<Cabinet, bool> predicate)
        {
            return Context.Cabinet.Where(predicate);
        }
        public void Delete(Cabinet entity)
        {
            Context.Cabinet.Remove(entity);
        }
        public void DeleteAll(IEnumerable<Cabinet> entity)
        {
            Context.Cabinet.RemoveRange(entity);
        }
        public void Update(Cabinet entity)
        {
            var entry = Context.Cabinet.Where(s => s.CabinetId == entity.CabinetId);
            Context.Cabinet.Attach(entity);
        }
        public bool AnyCabinet(Func<Cabinet, bool> predicate = null)
        {
            if (predicate == null)
                return Context.Cabinet.Any();
            else
                return Context.Cabinet.Any(predicate);
        }
        public async Task Save()
        {
            await Context.SaveChangesAsync();
        }
    }
}
