using PocketNurse.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PocketNurse.Repository
{
    public interface IPocketNurseRepository
    {
        IEnumerable<Patient> GetAllPatients();
        Task<Patient> FindPatient(string patientId, int cabinetId);
        IEnumerable<Patient> Find(Func<Patient, bool> predicate);
        void Add(Patient entity);
        void Delete(Patient entity);
        void DeleteAll(IEnumerable<Patient> entity);
        void Update(Patient entity);
        bool Any(Func<Patient, bool> predicate = null);

        IEnumerable<Cabinet> GetAllCabinets();
        void Add(Cabinet entity);
        Task<Cabinet> FindCabinet(int cabinetId);
        IEnumerable<Cabinet> Find(Func<Cabinet, bool> predicate);
        void Delete(Cabinet entity);
        void DeleteAll(IEnumerable<Cabinet> entity);
        void Update(Cabinet entity);
        bool Any(Func<Cabinet, bool> predicate = null);

        Task Save();
    }
}
