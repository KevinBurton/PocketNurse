using PocketNurse.Data;
using PocketNurse.Models;
using System.Collections.Generic;

namespace PocketNurse.Repository
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext Context;

        public UserRepository(ApplicationDbContext ctx)
        {
            Context = ctx;
        }
        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            return Context.Users;
        }
    }
}
