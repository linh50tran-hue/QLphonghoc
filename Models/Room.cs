using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace aznews.Models
{[Table("tblRoom")]
    public class Room
    {[Key]
    public int RoomID { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = "Available"; // Available, Maintenance
    public bool? IsActive { get; set; } = true;
    public int Quantity { get; set; } = 1;
    public string? Description { get; set; }
        
    }
}