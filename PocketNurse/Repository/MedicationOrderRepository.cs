using PocketNurse.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PocketNurse.Repository
{
    public class MedicationOrderRepository : IMedicationOrderRepository
    {
        private PocketNurseContext Context;

        public MedicationOrderRepository(PocketNurseContext ctx)
        {
            Context = ctx;
        }
        public IEnumerable<MedicationOrder> GetAll()
        {
            return Context.MedicationOrder;
        }
        public void Update(MedicationOrder entity)
        {
            var entry = Context.MedicationOrder.Where(s => s.MedicationOrderId == entity.MedicationOrderId);
            Context.MedicationOrder.Attach(entity);
        }
        public void Add(MedicationOrder entity)
        {
            Context.MedicationOrder.Add(entity);
        }
        public void Delete(MedicationOrder entity)
        {
            Context.MedicationOrder.Remove(entity);
        }
        public void DeleteAll(IEnumerable<MedicationOrder> entity)
        {
            Context.MedicationOrder.RemoveRange(entity);
        }
        public bool Any(Func<MedicationOrder, bool> predicate = null)
        {
            if (predicate == null)
                return Context.MedicationOrder.Any();
            else
                return Context.MedicationOrder.Any(predicate);
        }
        public void Save()
        {
            Context.SaveChanges();
        }
    }
}
