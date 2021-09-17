using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CheckInGWDN.Models;
using CheckInGWDN.Models.ViewModels;
using CheckInGWDN.Services.IRepository;
using CheckInGWDN.Utility;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckInGWDN.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    public class UserEventsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserEventsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = (SD.Administrator + "," + SD.Manager + "," + SD.Staff))]
        public async Task<IActionResult> Index()
        {
            var events = await _unitOfWork.Event.GetAllAsync();

            return View(events);
        }

        [Authorize(Roles = (SD.Administrator + "," + SD.Manager + "," + SD.Staff))]
        public async Task<IActionResult> Details(int id)
        {
            try
            {

                var eventInDb = await _unitOfWork.Event.GetAsync(id);
                if (eventInDb == null)
                {
                    TempData["ERROR"] = "Error: Event doesn't exists";
                    return RedirectToAction(nameof(Index));
                }

                return View(eventInDb);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Home");
            }
        }

        [HttpPost]
        [Authorize(Roles = (SD.Administrator + "," + SD.Manager + "," + SD.Staff))]
        public async Task<IActionResult> CheckIn(string studentId, int eventId)
        {
            var checkStdValid = await _unitOfWork.Student.GetFirstOrDefaultAsync(s => s.StudentId == studentId);

            var eventInDb = await _unitOfWork.Event.GetAsync(eventId);

            if (checkStdValid != null)
            {
                var studentEventValid = await _unitOfWork.UserEvents
                    .GetAllAsync(s => s.StudentId == checkStdValid.Id && s.EventId == eventId);

                if (studentEventValid.Any())
                {
                    return Json(new { success = false, valid = true, studentName = checkStdValid.Name, studentAva = checkStdValid.Avatar });
                }

                UserEvent userEventValid = new UserEvent()
                {
                    EventId = eventInDb.Id,
                    StudentId = checkStdValid.Id,
                    TimeStamp = DateTime.Now
                };

                await _unitOfWork.UserEvents.AddAsync(userEventValid);
                _unitOfWork.Save();
                return Json(new { success = true, valid = true, studentName = checkStdValid.Name, studentAva = checkStdValid.Avatar });
            }

            if (studentId == null)
            {
                return Json(new { success = false, valid = false });
            }

            if (studentId.Length % 4 != 0)
            {
                return Json(new { success = false, valid = false });
            }

            var studentInDb = await _unitOfWork.Student.GetFirstOrDefaultAsync(s => s.StudentId == Decrypt(studentId));

            if (studentInDb == null)
            {
                return Json(new { success = false, valid = false });
            }

            var studentEvent = await _unitOfWork.UserEvents
                .GetAllAsync(s => s.StudentId == studentInDb.Id && s.EventId == eventId);

            if (studentEvent.Any())
            {
                return Json(new { success = false, valid = true, studentName = studentInDb.Name, studentAva = studentInDb.Avatar });
            }

            UserEvent userEvent = new UserEvent()
            {
                EventId = eventInDb.Id,
                StudentId = studentInDb.Id,
                TimeStamp = DateTime.Now
            };
            
            await _unitOfWork.UserEvents.AddAsync(userEvent);
            _unitOfWork.Save();

            return Json(new { success = true, valid = true, studentName = studentInDb.Name, studentAva = studentInDb.Avatar });
        }

        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Report(int id)
        {
            try
            {
                var eventInDb = await _unitOfWork.Event.GetAsync(id);

                var listStudentId = await _unitOfWork.UserEvents.GetStudentsIdInEvent(id);
                
                var userEvents = await _unitOfWork.UserEvents
                    .GetAllAsync(
                        e => listStudentId.Any(stdId => stdId.Equals(e.StudentId)),
                        q => q.OrderBy(t => t.TimeStamp),
                        "Student,Event");

                StudentEventViewModel studentEventVm = new StudentEventViewModel
                {
                    Event = eventInDb,
                    UserEvents = userEvents
                };

                return View(studentEventVm);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Home");
            }
        }

        public static string Decrypt(string cipherText)
        {
            try
            {
                string EncryptionKey = Key.PrivateKey;
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch (Exception)
            {
                return "StudentWrong";
            }
        }

        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Excel(int id)
        {
            var listStudentId = await _unitOfWork.UserEvents.GetStudentsIdInEvent(id);

            //List<Student> students = await _db.Students.Where(s => listStudentId.Any(id => id.Equals(s.Id))).ToListAsync();

            var userEvents = await _unitOfWork.UserEvents
                .GetAllAsync(
                    e => listStudentId.Any(stdId => stdId.Equals(e.StudentId)),
                    q => q.OrderBy(t => t.TimeStamp),
                    "Student,Event");

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Student");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "No.";
                worksheet.Cell(currentRow, 2).Value = "Student Id";
                worksheet.Cell(currentRow, 3).Value = "Student Name";

                foreach (var student in userEvents)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = currentRow - 1;
                    worksheet.Cell(currentRow, 2).Value = student.Student.StudentId;
                    worksheet.Cell(currentRow, 3).Value = student.Student.Name;
                }

                await using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "student.xlsx");
                }
            }
        }
    }
}