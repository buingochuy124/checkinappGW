using CheckInGWDN.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CheckInGWDN.Services.IRepository;
using Microsoft.AspNetCore.Hosting;

namespace CheckInGWDN.Areas.Unauthenticated.Controllers
{
    [Area("Unauthenticated")]
    public class CheckQrController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckQrController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        //Post :: CheckQr
        [HttpPost]
        public async Task<IActionResult> GetQrCode(string studentId)
        {
            {
                var doesStudentExists = await _unitOfWork.Student.GetFirstOrDefaultAsync(e => e.StudentId == studentId);
                if (doesStudentExists == null)
                {
                    return PartialView("_AJaxGetQr", null);
                }

                return PartialView("_AJaxGetQr", doesStudentExists);
            }
        }
    }
}