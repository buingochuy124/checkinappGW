using CheckInGWDN.Models;
using CheckInGWDN.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CheckInGWDN.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (_db.Roles.Any(r => r.Name == SD.Administrator)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.Administrator)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Manager)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Staff)).GetAwaiter().GetResult();

            var result = _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "buingochuy124@gmail.com",
                Email = "admin@gmail.com",
                FullName = "Bui Ngoc Huy",
                EmailConfirmed = true,
            }, "Password@123").GetAwaiter().GetResult();

            if (result.Succeeded)
            {
                IdentityUser user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == "buingochuy124@gmail.com");
                await _userManager.AddToRoleAsync(user, SD.Administrator);
            }
        }
    }
}