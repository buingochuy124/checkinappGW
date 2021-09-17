using System;
using System.Threading.Tasks;

namespace CheckInGWDN.Services.IRepository
{
    public interface IUnitOfWork: IDisposable
    {
        public IUserRepository User { get; }
        public IEventsRepository Event { get; }
        public IStudentRepository Student { get; }
         public IDownloadRepository Download { get; }
         public IUserEventsRepository UserEvents { get; }
        void Save();
    }
}