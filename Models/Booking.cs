using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace aznews.Models
{
    [Table("tblBooking")]
public class Booking
{
    [Key]
    [ForeignKey("RoomID")]

    public Room? Room { get; set; }
    public int BookingID { get; set; } 
   [ForeignKey("UserID")]
    public int UserID { get; set; }
    public int RoomID { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Cancelled
    public string? RejectReason { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
}