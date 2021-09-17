using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CheckInGWDN.Data;
using CheckInGWDN.Models;
using CheckInGWDN.Services.IRepository;

namespace CheckInGWDN.Services
{
    public class UserRepository: RepositoryAsync<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task Update(ApplicationUser applicationUser)
        {
            var user = await GetFirstOrDefaultAsync(u => u.Id == applicationUser.Id);
            if (user != null)
            {
                _db.ApplicationUsers.Update(applicationUser);
            }
        }
    }
}