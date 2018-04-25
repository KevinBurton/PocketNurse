using PocketNurse.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PocketNurse.Repository
{
    public class PatientAllergyRepository : IPatientAllergyRepository
    {
        private PocketNurseContext Context;

        public PatientAllergyRepository(PocketNurseContext ctx)
        {
            Context = ctx;
        }
        public IEnumerable<PatientAllergy> GetAll()
        {
            return Context.PatientAllergy;
        }
        public void Add(PatientAllergy entity)
        {
            Context.PatientAllergy.Add(entity);
        }
        public void Delete(PatientAllergy entity)
        {
            Context.PatientAllergy.Remove(entity);
        }
        public void DeleteAll(IEnumerable<PatientAllergy> entity)
        {
            Context.PatientAllergy.RemoveRange(entity);
        }
        public void Update(PatientAllergy entity)
        {
            var entry = Context.PatientAllergy.Where(s => s.PatientAllergyId == entity.PatientAllergyId);
            Context.PatientAllergy.Attach(entity);
        }
        public bool Any(Func<PatientAllergy, bool> predicate = null)
        {
            if (predicate == null)
                return Context.PatientAllergy.Any();
            else
                return Context.PatientAllergy.Any(predicate);
        }
        public void Save()
        {
            Context.SaveChanges();
        }
    }
}
