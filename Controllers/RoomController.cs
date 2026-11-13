using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using aznews.Models;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions;
namespace aznews.Controllers
{public class RoomController : Controller
    {
        private readonly DataContext _context;

        public RoomController(DataContext context)
        {
            _context = context;
        }
        
        // GET: /Room/Index
        public async Task<IActionResult> Index()
        {
            var listRoom = await _context.Rooms
                                         .Where(r => r.IsActive == true)
                                         .OrderBy(r => r.RoomName)
                                         .ToListAsync();
            return View(listRoom);
        }
        

       // GET: /Room/Details/5
        // GET: /Room/Details/8
public async Task<IActionResult> Details(int? id)
{
    if (id == null) return NotFound();

    var room = await _context.Rooms.FirstOrDefaultAsync(m => m.RoomID == id && m.IsActive == true);
    if (room == null) return NotFound();

    var upcomingBookings = await _context.Bookings
        .Where(b => b.RoomID == id && b.Status != "Rejected" && b.EndTime >= DateTime.Now)
        .OrderBy(b => b.StartTime)
        .ToListAsync();

    var roomDevices = await _context.Devices
        .Where(d => d.RoomID == id && d.IsActive == true)
        .OrderBy(d => d.DeviceName)
        .ToListAsync();

    ViewBag.UpcomingBookings = upcomingBookings;
    ViewBag.RoomDevices = roomDevices;

    return View(room);
}
        }
    }
