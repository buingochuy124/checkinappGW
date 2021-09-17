using System.Threading.Tasks;
using CheckInGWDN.Models;

namespace CheckInGWDN.Services.IRepository
{
    public interface IUserRepository: IRepositoryAsync<ApplicationUser>
    {
        Task Update(ApplicationUser applicationUser);
    }
}