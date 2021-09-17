using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckInGWDN.Data;
using CheckInGWDN.Models;
using CheckInGWDN.Services.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CheckInGWDN.Services
{
    public class EventsRepository: RepositoryAsync<Event>,IEventsRepository
    {
        private readonly ApplicationDbContext _db;

        public EventsRepository(ApplicationDbContext db) : base(db)
        {
            this._db = db;
        }

        public async Task Update(Event @event)
        {
            var objInDb = await _db.Events.FirstOrDefaultAsync(e => e.Id == @event.Id);
            if (objInDb != null)
            {
                if (!await CheckExistEvent(@event.Name) && objInDb.Name != @event.Name)
                {
                    objInDb.Name = @event.Name;
                    objInDb.Description = @event.Description;
                }
            }
        }
        public async Task<bool> ChangeStatusEvent(int id)
        {
            var @event = await GetAsync(id);
            if (@event == null)
            {
                return false;
            }
            @event.status = @event.status != true;
            return true;
        }

        public async Task<bool> CheckExistEvent(string name)
        {
            return await _db.Events.AnyAsync(e => e.Name.Contains(name));
        }
    }
}