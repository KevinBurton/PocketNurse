using PocketNurse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketNurse.Repository
{
    public interface IUserRepository
    {
        IEnumerable<ApplicationUser> GetAllUsers();
    }
}
