using PocketNurse.Models;
using System;
using System.Collections.Generic;

namespace PocketNurse.Repository
{
    public interface IPatientRepository
    {
        IEnumerable<Patient> GetAll();
        Patient Find(string patientId);
        void Add(Patient entity);
        void Delete(Patient entity);
        void DeleteAll(IEnumerable<Patient> entity);
        void Update(Patient entity);
        bool Any(Func<Patient, bool> predicate = null);
        void Save();
    }
}
