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
    public class IncidentController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public IncidentController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Incident/Create (Form gửi báo cáo sự cố)
        public async Task<IActionResult> Create(int? roomId)
        {
            var rooms = await _context.Rooms
                .Where(r => r.IsActive == true)
                .OrderBy(r => r.RoomName)
                .ToListAsync();

            ViewBag.RoomList = new SelectList(rooms, "RoomID", "RoomName", roomId);

            var devices = new List<Device>();
            if (roomId.HasValue && roomId.Value > 0)
            {
                devices = await _context.Devices
                    .Where(d => d.IsActive == true && d.RoomID == roomId.Value)
                    .ToListAsync();
            }
            ViewBag.DeviceList = new SelectList(devices, "DeviceID", "DeviceName");

            return View(new Incident { RoomID = roomId ?? 0 });
        }

        // API lấy thiết bị theo RoomID khi User đổi phòng trên dropdown
        [HttpGet]
        public async Task<IActionResult> GetDevicesByRoom(int roomId)
        {
            var devices = await _context.Devices
                .Where(d => d.IsActive == true && d.RoomID == roomId)
                .Select(d => new
                {
                    deviceID = d.DeviceID,
                    deviceName = $"{d.DeviceCode} - {d.DeviceName}"
                })
                .ToListAsync();

            return Json(devices);
        }

        // POST: /Incident/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Incident incident, IFormFile? imageFile)
        {
            // Lấy UserID từ Session (nếu chưa có Session UserID thì lấy UserID hợp lệ đầu tiên trong DB để tránh lỗi khóa ngoại)
            var sessionUserId = HttpContext.Session.GetInt32("UserID");
            if (sessionUserId.HasValue && sessionUserId.Value > 0)
            {
                incident.UserID = sessionUserId.Value;
            }
            else
            {
                var defaultUser = await _context.AdminUsers.FirstOrDefaultAsync();
                incident.UserID = defaultUser?.UserID ?? 1;
            }

            if (incident.RoomID <= 0)
            {
                ModelState.AddModelError("RoomID", "Vui lòng chọn phòng học xảy ra sự cố.");
            }

            // Xử lý lưu file ảnh minh chứng nếu người dùng có chọn ảnh
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "incidents");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                incident.ImageUrl = "/uploads/incidents/" + uniqueFileName;
            }

            incident.Status = "Pending";
            incident.CreatedDate = DateTime.Now;

            // Bỏ qua validate các Navigation Properties
            ModelState.Remove("User");
            ModelState.Remove("Room");
            ModelState.Remove("Device");

            if (ModelState.IsValid)
            {
                _context.Incidents.Add(incident);

                // Nếu báo cáo gắn với một thiết bị cụ thể, cập nhật trạng thái thiết bị sang "Broken"
                if (incident.DeviceID.HasValue && incident.DeviceID.Value > 0)
                {
                    var device = await _context.Devices.FindAsync(incident.DeviceID.Value);
                    if (device != null)
                    {
                        device.Status = "Broken";
                        _context.Devices.Update(device);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Gửi báo cáo sự cố thành công! Bộ phận kỹ thuật sẽ sớm tiếp nhận và xử lý.";
                return RedirectToAction("Details", "Room", new { id = incident.RoomID });
            }

            var rooms = await _context.Rooms.Where(r => r.IsActive == true).OrderBy(r => r.RoomName).ToListAsync();
            ViewBag.RoomList = new SelectList(rooms, "RoomID", "RoomName", incident.RoomID);
            return View(incident);
        }

        // GET: /Incident/Index (Danh sách sự cố - Admin xem tất cả, User xem của mình)
        // GET: /Incident/Index (Lịch sử báo cáo sự cố của User đang đăng nhập)
public async Task<IActionResult> Index()
{
    var userId = HttpContext.Session.GetInt32("UserID");

    var query = _context.Incidents
        .Include(i => i.Room)
        .Include(i => i.Device)
        .AsQueryable();

    // Chỉ lấy báo cáo của chính User đang đăng nhập
    if (userId.HasValue && userId.Value > 0)
    {
        query = query.Where(i => i.UserID == userId.Value);
    }

    var list = await query.OrderByDescending(i => i.CreatedDate).ToListAsync();
    return View(list);
}

// POST: /Incident/Delete/5 (Cho phép User thu hồi báo cáo khi chưa được xử lý)
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id)
{
    var incident = await _context.Incidents.FindAsync(id);
    if (incident != null && incident.Status == "Pending")
    {
        // Phục hồi trạng thái thiết bị về "Good" nếu có gắn thiết bị
        if (incident.DeviceID.HasValue && incident.DeviceID.Value > 0)
        {
            var device = await _context.Devices.FindAsync(incident.DeviceID.Value);
            if (device != null)
            {
                device.Status = "Good";
            }
        }

        // Xóa file ảnh đính kèm trong wwwroot nếu có
        if (!string.IsNullOrEmpty(incident.ImageUrl))
        {
            var fullPath = Path.Combine(_env.WebRootPath, incident.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        _context.Incidents.Remove(incident);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã thu hồi báo cáo sự cố thành công!";
    }
    else
    {
        TempData["ErrorMessage"] = "Không thể thu hồi báo cáo đã được tiếp nhận hoặc xử lý.";
    }

    return RedirectToAction(nameof(Index));
}
    }
}