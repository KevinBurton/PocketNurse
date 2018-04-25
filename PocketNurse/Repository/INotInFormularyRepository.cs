using PocketNurse.Models;
using System;
using System.Collections.Generic;

namespace PocketNurse.Repository
{
    public interface INotInFormularyRepository
    {
        IEnumerable<NotInFormulary> GetAll();
        void Add(NotInFormulary entity);
        void Delete(NotInFormulary entity);
        void DeleteAll(IEnumerable<NotInFormulary> entity);
        void Update(NotInFormulary entity);
        bool Any(Func<NotInFormulary, bool> predicate = null);
        void Save();
    }
}
