using PocketNurse.Models;
using System;
using System.Collections.Generic;

namespace PocketNurse.Repository
{
    public interface IPatientAllergyRepository
    {
        IEnumerable<PatientAllergy> GetAll();
        void Add(PatientAllergy entity);
        void Delete(PatientAllergy entity);
        void DeleteAll(IEnumerable<PatientAllergy> entity);
        void Update(PatientAllergy entity);
        bool Any(Func<PatientAllergy, bool> predicate = null);
        void Save();
    }
}
