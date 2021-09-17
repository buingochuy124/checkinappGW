using System.Threading.Tasks;
using CheckInGWDN.Models;

namespace CheckInGWDN.Services.IRepository
{
    public interface IUserEventsRepository: IRepositoryAsync<UserEvent>
    {
        Task<int[]> GetStudentsIdInEvent(int id);
    }
}