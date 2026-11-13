using aznews.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace aznews.Models
{
   public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    public DbSet<AdminMenu> AdminMenus { get; set; }
    public DbSet<AdminUser> AdminUsers   { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<UserMenu> UserMenus { get; set; }
}
}