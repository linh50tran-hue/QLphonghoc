using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace aznews.Models
{
   [Table("tblIncident")]
public class Incident
{
    [Key]
        public int IncidentID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng học")]
        public int RoomID { get; set; }

        public int? DeviceID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả sự cố")]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending (Chờ xử lý), InProgress (Đang sửa), Resolved (Đã khắc phục)

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Các thuộc tính điều hướng (Navigation Properties)
        [ForeignKey("UserID")]
        public AdminUser? User { get; set; }

        [ForeignKey("RoomID")]
        public Room? Room { get; set; }

        [ForeignKey("DeviceID")]
        public Device? Device { get; set; }
}
}