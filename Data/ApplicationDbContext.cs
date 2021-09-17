using System;
using System.Collections.Generic;
using System.Text;
using CheckInGWDN.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CheckInGWDN.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }
        public DbSet<Download> Downloads { get; set; }
    }
}