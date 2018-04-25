using PocketNurse.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PocketNurse.Repository
{
    public class NotInFormularyRepository : INotInFormularyRepository
    {
        private PocketNurseContext Context;

        public NotInFormularyRepository(PocketNurseContext ctx)
        {
            Context = ctx;
        }
        public IEnumerable<NotInFormulary> GetAll()
        {
            return Context.NotInFormulary;
        }
        public void Add(NotInFormulary entity)
        {
            Context.NotInFormulary.Add(entity);
        }
        public void Delete(NotInFormulary entity)
        {
            Context.NotInFormulary.Remove(entity);
        }
        public void DeleteAll(IEnumerable<NotInFormulary> entity)
        {
            Context.NotInFormulary.RemoveRange(entity);
        }
        public void Update(NotInFormulary entity)
        {
            var entry = Context.NotInFormulary.Where(s => s._id == entity._id);
            Context.NotInFormulary.Attach(entity);
        }
        public bool Any(Func<NotInFormulary, bool> predicate = null)
        {
            if (predicate == null)
                return Context.NotInFormulary.Any();
            else
                return Context.NotInFormulary.Any(predicate);
        }
        public void Save()
        {
            Context.SaveChanges();
        }
    }
}
