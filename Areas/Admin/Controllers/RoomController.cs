using Microsoft.AspNetCore.Mvc;
using aznews.Models;

namespace aznews.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoomController : Controller
    {
        private readonly DataContext _context;

        public RoomController(DataContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH PHÒNG HỌC
        public IActionResult Index()
        {
            var rList = _context.Rooms.OrderBy(r => r.RoomID).ToList();
            return View(rList);
        }

        // 2. XÓA PHÒNG HỌC (GET)
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var rm = _context.Rooms.Find(id);
            if (rm == null)
            {
                return NotFound();
            }
            return View(rm);
        }

        // 2. XÓA PHÒNG HỌC (POST)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var delRoom = _context.Rooms.Find(id);
            if (delRoom == null)
            {
                return NotFound();
            }

            bool hasBooking = _context.Bookings.Any(b => b.RoomID == id);
            if (hasBooking)
            {
                TempData["Error"] = "Phòng này đang có lịch đặt phòng nên không thể xóa!";
                return RedirectToAction("Index");
            }

            _context.Rooms.Remove(delRoom);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // 3. THÊM MỚI PHÒNG HỌC (GET)
        public IActionResult Create()
        {
            return View();
         
        }
        // 3. THÊM MỚI PHÒNG HỌC (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room rm)
        {
            if (ModelState.IsValid)
            {
                _context.Rooms.Add(rm);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rm);
        }
        

        // 4. CHỈNH SỬA PHÒNG HỌC (GET)
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var rm = _context.Rooms.Find(id);
            if (rm == null)
            {
                return NotFound();
            }
            return View(rm);
        }

        // 4. CHỈNH SỬA PHÒNG HỌC (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Room rm)
        {
            if (ModelState.IsValid)
            {
                _context.Rooms.Update(rm);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rm);
        }
    }
}