using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace aznews.Models
{
   [Table("tblDevice")]
public class Device
{
    [Key]
    public int DeviceID { get; set; }
    public string DeviceCode { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public int RoomID { get; set; }
    public string Status { get; set; } = "Good"; // Good, Broken, Maintenance
    public bool? IsActive { get; set; } = true;
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
}
}
