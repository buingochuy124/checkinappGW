using System.Threading.Tasks;
using CheckInGWDN.Models;

namespace CheckInGWDN.Services.IRepository
{
    public interface IEventsRepository: IRepositoryAsync<Event>
    {
        Task Update(Event @event);
        Task<bool> ChangeStatusEvent(int id);

        Task<bool> CheckExistEvent(string name);
    }
}