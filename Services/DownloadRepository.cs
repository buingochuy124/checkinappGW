using CheckInGWDN.Data;
using CheckInGWDN.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckInGWDN.Services.IRepository;
using Microsoft.AspNetCore.Hosting;

namespace CheckInGWDN.Services
{
    public class DownloadRepository : RepositoryAsync<Download>, IDownloadRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DownloadRepository(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment) : base(db)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task AddNewDownload(string fileName)
        {
     
            await _db.Downloads.AddAsync(new Download()
            {
                Name = $"{fileName}",
                ZipPath = $"{fileName}-at-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}"
            });
        }
    }
}