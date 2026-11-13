using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aznews.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace aznews.Controllers
{
    public class BookingController : Controller
    {
        
        private readonly DataContext _context;

        public BookingController(DataContext context)
        {
            _context = context;
        }

public IActionResult Index()
        {var userId = HttpContext.Session.GetInt32("UserID");

    var query = _context.Bookings
        .Include(b => b.Room)
        .AsQueryable();

    if (userId.HasValue && userId.Value > 0)
    {
        query = query.Where(b => b.UserID == userId.Value);
    }

    var bookings = query
        .OrderByDescending(b => b.CreatedDate)
        .ToList();

    return View(bookings);

        }
        // GET: /Booking/Create?roomId=1
        public IActionResult Create(int? roomId)
        {
            var availableRooms = _context.Rooms
                .Where(r => r.IsActive == true && r.Status != "Maintenance")
                .OrderBy(r => r.RoomName)
                .ToList();

            ViewBag.RoomList = new SelectList(availableRooms, "RoomID", "RoomName", roomId);

            // Mặc định gợi ý giờ bắt đầu là tròn giờ tiếp theo, kéo dài 2 tiếng
            var now = DateTime.Now;
            var defaultStart = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1);

            var model = new Booking
            {
                RoomID = roomId ?? 0,
                StartTime = defaultStart,
                EndTime = defaultStart.AddHours(2)
            };

            return View(model);
        }

        // POST: /Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking booking)
        {
            var availableRooms = _context.Rooms
                .Where(r => r.IsActive == true && r.Status != "Maintenance")
                .OrderBy(r => r.RoomName)
                .ToList();
            ViewBag.RoomList = new SelectList(availableRooms, "RoomID", "RoomName", booking.RoomID);

            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError("EndTime", "Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
                return View(booking);
            }

            // Kiểm tra trùng lịch với các đơn chưa bị từ chối
            bool isConflict = _context.Bookings.Any(b =>
                b.RoomID == booking.RoomID &&
                b.Status != "Rejected" &&
                booking.StartTime < b.EndTime &&
                booking.EndTime > b.StartTime);

            if (isConflict)
            {
                ModelState.AddModelError("", "Phòng này đã có lịch đặt trùng trong khoảng thời gian bạn chọn!");
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                // Nếu đã có Session đăng nhập thì lấy UserID, tạm thời gán null hoặc 1 nếu chưa bật Login
                booking.UserID = HttpContext.Session.GetInt32("UserID") ?? 1;
                booking.Status = "Pending";
                booking.CreatedDate = DateTime.Now;

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Gửi yêu cầu đặt phòng thành công! Vui lòng chờ phê duyệt.";
                return RedirectToAction(nameof(Index));
            }

            return View(booking);
        }
        [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(int id)
    {
        var booking = _context.Bookings.Find(id);
        if (booking != null && booking.Status == "Pending")
        {
        _context.Bookings.Remove(booking);
        _context.SaveChanges();
        TempData["SuccessMessage"] = "Đã hủy yêu cầu đặt phòng thành công!";
        }
        else
        {
            TempData["ErrorMessage"] = "Không thể hủy yêu cầu đặt phòng này!";
        }
            return RedirectToAction(nameof(Index));
}
    }
    }
