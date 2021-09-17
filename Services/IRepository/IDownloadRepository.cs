using System.Threading.Tasks;
using CheckInGWDN.Models;

namespace CheckInGWDN.Services.IRepository
{
    public interface IDownloadRepository: IRepositoryAsync<Download>
    {
        Task AddNewDownload(string fileName);
    }
}