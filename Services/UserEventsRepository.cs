using System.Linq;
using System.Threading.Tasks;
using CheckInGWDN.Data;
using CheckInGWDN.Models;
using CheckInGWDN.Services.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CheckInGWDN.Services
{
    public class UserEventsRepository: RepositoryAsync<UserEvent>, IUserEventsRepository
    {
        private readonly ApplicationDbContext _db;
        public UserEventsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<int[]> GetStudentsIdInEvent(int id)
        {
            var listStudentId = await _db.UserEvents
                .Where(e => e.EventId == id)
                .Select(s => s.StudentId).ToArrayAsync();
            return listStudentId; 
        }
    }
}