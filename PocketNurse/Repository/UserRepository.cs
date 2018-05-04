using PocketNurse.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketNurse.Repository
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext Context;

        public UserRepository(ApplicationDbContext ctx)
        {
            Context = ctx;
        }
    }
}
