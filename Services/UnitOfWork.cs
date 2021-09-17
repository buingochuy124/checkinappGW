using System.Threading.Tasks;
using CheckInGWDN.Data;
using CheckInGWDN.Services.IRepository;
using Microsoft.AspNetCore.Hosting;

namespace CheckInGWDN.Services
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            Event = new EventsRepository(_db);
            Student = new StudentRepository(_db);
            User = new UserRepository(_db);
            Download = new DownloadRepository(_db, webHostEnvironment);
            UserEvents = new UserEventsRepository(_db);
        }
        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
            this.Dispose();
        }
        
        public IDownloadRepository Download { get; }
        public IStudentRepository Student { get; }
        public IUserRepository User { get; }
        public IUserEventsRepository UserEvents { get; }
        public IEventsRepository Event { get; private set; }
    }
}