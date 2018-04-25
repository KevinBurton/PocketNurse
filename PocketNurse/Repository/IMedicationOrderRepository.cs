using PocketNurse.Models;
using System;
using System.Collections.Generic;

namespace PocketNurse.Repository
{
    public interface IMedicationOrderRepository
    {
        IEnumerable<MedicationOrder> GetAll();
        void Add(MedicationOrder entity);
        void Delete(MedicationOrder entity);
        void DeleteAll(IEnumerable<MedicationOrder> entity);
        void Update(MedicationOrder entity);
        bool Any(Func<MedicationOrder, bool> predicate = null);
        void Save();
    }
}
