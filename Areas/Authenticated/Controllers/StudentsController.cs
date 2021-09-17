using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CheckInGWDN.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using CheckInGWDN.Utility;
using QRCoder;
using System.Drawing;
using System.Security.Cryptography;
using CheckInGWDN.Services.IRepository;

namespace CheckInGWDN.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    [Authorize(Roles = (SD.Administrator))]
    public class StudentsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUnitOfWork _unitOfWork;

        private static string _fileName;

        public StudentsController(
            IWebHostEnvironment webHostEnvironment,
            IUnitOfWork unitOfWork
            )
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(List<Student> students = null)
        {
            students = students == null ? new List<Student>() : students;

            return View(students);
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file, [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                if (file == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                string fileName = $"{webHostEnvironment.WebRootPath}//files//{file.FileName}";

                StudentsController._fileName = file.FileName.Split(".")[0];

                await using (FileStream fileStream = System.IO.File.Create(fileName))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Flush();
                }

                var students = this.GetStudentList(file.FileName);
                return Index(students.Result);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Home");
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ERROR"] = "Error: URL doesn't have student Id";
                return RedirectToAction(nameof(Index));
            }
            var student = await _unitOfWork.Student.GetFirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                TempData["ERROR"] = "Error: Student doesn't exists";
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student student)
        {
            var webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var studentInDb = await _unitOfWork.Student.GetAsync(student.Id);

            var doesStudentExists = await _unitOfWork.Student.GetAllAsync(e => e.StudentId == student.StudentId);

            if (doesStudentExists.Any() && studentInDb.StudentId != student.StudentId)
            {
                TempData["ERROR"] = "Error: Student ID already exists";
                return View(student);
            }

            studentInDb.StudentId = student.StudentId;
            studentInDb.Name = student.Name;

            if (files.Count > 0)
            {
                //upload file
                var uploads = Path.Combine(webRootPath, "Avatar");
                var extension = Path.GetExtension(files[0].FileName);

                await using (var fileStream = new FileStream(Path.Combine(uploads, student.StudentId + extension), FileMode.Create))
                {
                    await files[0].CopyToAsync(fileStream);
                }
                try
                {
                    studentInDb.Avatar = @"\avatar\" + student.StudentId + extension;
                }
                catch (Exception)
                {
                    studentInDb.Avatar = @"/avatar/" + student.StudentId + extension;
                }
            }

            _unitOfWork.Save();
            TempData["SUCCESS"] = "Success: Update Student successfully";
            return RedirectToAction("Edit", new { id = studentInDb.Id.ToString() });
        }

        private async Task<List<Student>> GetStudentList(string fName)
        {
            var errorCount = 0;
            var studentCount = 0;
            var students = new List<Student>();

            try
            {
                var fileName = $"{Directory.GetCurrentDirectory()}{@"/wwwroot/files"}" + "//" + fName;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                await using var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                });

                while (reader.Read())
                {
                    var doesStudentExist = await _unitOfWork.Student.GetAllAsync(
                        s => s.StudentId == reader.GetValue(0).ToString()
                    );

                    if (doesStudentExist.Any())
                    {
                        ViewData["MessageStudent"] = "Error: Student " + reader.GetValue(1) + " with student Id: " + reader.GetValue(0) + " already exists";
                        errorCount++;
                    }
                    else
                    {
                        students.Add(new Student()
                        {
                            StudentId = reader.GetValue(0).ToString(),
                            Name = reader.GetValue(1).ToString(),
                        });
                        studentCount++;
                    }
                }
                await _unitOfWork.Student.AddRangeAsync(students);
            }
            catch (Exception)
            {
                var fileName = $"{Directory.GetCurrentDirectory()}{@"/wwwroot/files"}" + "\\" + fName;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                await using var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                });

                while (reader.Read())
                {
                    var doesStudentExist = await _unitOfWork.Student.GetAllAsync(
                        s => s.StudentId == reader.GetValue(0).ToString()
                    );
                    if (doesStudentExist.Any())
                    {
                        TempData["ERROR"] = "Student " + reader.GetValue(1) + " with student Id: " + reader.GetValue(0) + " already exists";
                        errorCount++;
                    }
                    else
                    {
                        students.Add(new Student()
                        {
                            StudentId = reader.GetValue(0).ToString(),
                            Name = reader.GetValue(1).ToString(),
                        });
                        studentCount++;
                    }
                }
                await _unitOfWork.Student.AddRangeAsync(students);
            }
            if (errorCount > 0)
            {
                TempData["WARNING"] = "There are " + errorCount + " students have same student Id. Please check again!";
            }
            _unitOfWork.Save();
            ViewData["SUCCESS"] = "There are " + studentCount + " students have been import to system!";
            return students;
        }

        public IActionResult ListStudent()
        {
            return View(_unitOfWork.Student.GetAllAsync().Result);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> CreateQrCode()
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var listStudent = await _unitOfWork.Student.GetStudentsWithNullQr(150);
            if (!listStudent.Any())
            {
                TempData["ERROR"] = "Error: No data student. Please import student list!";
                return RedirectToAction(nameof(ListStudent));
            }
            else
            {
                // TODO: Download Repository
                await _unitOfWork.Download.AddNewDownload(_fileName);
            }

            var num = await _unitOfWork.Student.CountStudentHasQr();
            foreach (var student in listStudent)
            {
                var studentId = Encrypt(student.StudentId);
                QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(studentId, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                await BitmapToBytesCode(qrCodeImage, student, num);
                num++;
            }
            _unitOfWork.Student.UpdateListStudent(listStudent);
            _unitOfWork.Save();
            TempData["SUCCESS"] = "Success: Create Student Successfully";
            return RedirectToAction(nameof(ListStudent));
        }

        [NonAction]
        private async Task BitmapToBytesCode(Bitmap image, Student student, int num)
        {
            var webRootPath = _webHostEnvironment.WebRootPath;
            await using var stream = new MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            var img = Image.FromStream(stream);
            var qrPath = "/qr/" + $"{StudentsController._fileName}-at-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}/";
            var zipPath = webRootPath + qrPath;
            if (!Directory.Exists(zipPath))
                Directory.CreateDirectory(zipPath);
            student.Qr = $"{qrPath}{num}. {student.Name}.png";
            img.Save(zipPath + num.ToString() + ". " + student.Name + ".png", System.Drawing.Imaging.ImageFormat.Png);
            stream.ToArray();
            stream.Close();
        }

        private static string Encrypt(string clearText)
        {
            var EncryptionKey = Key.PrivateKey;
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        //GET :: Details
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var student = await _unitOfWork.Student.GetAsync(id);
                if (student == null)
                {
                    TempData["ERROR"] = "Error: Student doesn't exist";
                    return RedirectToAction(nameof(ListStudent));
                }

                return View(student);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Home");
            }
        }

        //POST :: DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _unitOfWork.Student.GetAsync(id);

            if (student == null)
            {
                TempData["ERROR"] = "Error: Student doesn't exist";
                return RedirectToAction(nameof(ListStudent));
            }

            await _unitOfWork.Student.RemoveAsync(student.Id);
            _unitOfWork.Save();
            TempData["SUCCESS"] = $"Success: Delete {student.Name} successfully";
            return RedirectToAction(nameof(ListStudent));
        }
    }
}