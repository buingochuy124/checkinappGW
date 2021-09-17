using System.Collections.Generic;
using System.Threading.Tasks;
using CheckInGWDN.Models;

namespace CheckInGWDN.Services.IRepository
{
    public interface IStudentRepository: IRepositoryAsync<Student>
    {
        Task<List<Student>> GetStudentsWithNullQr(int numberOfStudent);
        
        Task<int> CountStudentHasQr();

        void UpdateListStudent(List<Student> students);
    }
}