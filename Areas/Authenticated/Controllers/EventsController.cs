using System.Threading.Tasks;
using CheckInGWDN.Models;
using CheckInGWDN.Services.IRepository;
using CheckInGWDN.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckInGWDN.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    public class EventsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public EventsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //GET :: Index
        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Index()
        {
            var events = await  _unitOfWork.Event.GetAllAsync();
            return View(events);
        }

        //GET :: Create
        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public IActionResult Create()
        {
            var events = new Event();
            return View(events);
        }
    
        //Post :: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Event.CheckExistEvent(@event.Name))
                {
                    TempData["ERROR"] = $"Error: Event {@event.Name} already exists";
                    return View(@event);
                }
                await _unitOfWork.Event.AddAsync(@event);
                _unitOfWork.Save();
                TempData["SUCCESS"] = $"Success: Create {@event.Name} successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["WARNING"] = "Warning: Please try again create";
            return RedirectToAction(nameof(Create));
        }
        
        //GET :: Details
        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Details(int id)
        {
            var @event = await _unitOfWork.Event.GetAsync(id);
            if (@event == null)
            {
                TempData["ERROR"] = "Error: Event doesn't exists";
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }
        
        //GET :: Edit
        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Edit(int id)
        {
            var @event = await _unitOfWork.Event.GetAsync(id);
            if (@event == null)
            {
                TempData["ERROR"] = "Error: Event doesn't exists";
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }
        
        //POST :: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Edit(Event @event)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.Event.CheckExistEvent(@event.Name))
                {
                    TempData["ERROR"] = $"Error: Event {@event.Name} are already exists";
                    return View(@event);
                }
                await _unitOfWork.Event.Update(@event);
                TempData["SUCCESS"] = $"Success: Edit {@event.Name} successfully";
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            TempData["WARNING"] = "Warning: Please try again update";
            return View(@event);
        }
        //
        // //POST :: DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Delete(int id)
        {
            await _unitOfWork.Event.RemoveAsync(id);
            _unitOfWork.Save();
            TempData["SUCCESS"] = "Success: Delete successfully";
            return RedirectToAction(nameof(Index));
        }
        
        //POST :: Status
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = (SD.Administrator + "," + SD.Manager))]
        public async Task<IActionResult> Status(int id)
        {
            if (await _unitOfWork.Event.ChangeStatusEvent(id))
            {
                TempData["SUCCESS"] = "Success: Change status successfully";
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            TempData["ERROR"] = "Error: Can't change status";
            return RedirectToAction(nameof(Index));
        }
    }
}