using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CheckInGWDN.Models;
using CheckInGWDN.Services.IRepository;
using CheckInGWDN.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CheckInGWDN.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole(SD.Administrator))
            {
                var claimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var users = await _unitOfWork.User.GetAllAsync(u => u.Id != claim.Value);

                return View(users);
            }
            else
            {
                var claimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userListDb = await _unitOfWork.User.GetAllAsync(u => u.Id != claim.Value);
                foreach (var user in userListDb)
                {
                    var userTemp = await _userManager.FindByIdAsync(user.Id);
                    var roleTemp = await _userManager.GetRolesAsync(userTemp);
                    user.Role = roleTemp.FirstOrDefault();
                }

                var userList = userListDb.Where(u => u.Role == SD.Manager || u.Role == SD.Staff);
                return View(userList);
            }
        }

        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var applicationUser = await _unitOfWork.User.GetFirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _unitOfWork.User.GetFirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now;

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _unitOfWork.User.GetAsync(id);
           if (user == null)
            {
                return View(null);
            }

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _unitOfWork.User.GetAsync(id);
            if (user == null)
            {
                return View();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var isUserExists = await _unitOfWork.User.GetAllAsync(u => u.Id == user.Id);
                var isEmailExists = await _unitOfWork.User.GetAllAsync(u => u.Email == user.Email);
                if (isEmailExists.Any() && isUserExists.First().Email != user.Email)
                {
                    ViewData["Message"] = "Error: User with this email already exists";
                    return View(user);
                }
                
                var userFromDb = await _unitOfWork.User.GetAsync(user.Id);
                userFromDb.FullName = user.FullName;
                userFromDb.Email = user.Email;
                userFromDb.PhoneNumber = user.PhoneNumber;
                
                await _unitOfWork.User.Update(userFromDb);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
    }
}