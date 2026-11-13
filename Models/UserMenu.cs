using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace aznews.Models
{
 
        [Table("tblUserMenu")]
    public class UserMenu
    {
        [Key]
        public int MenuID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int ItemOrder { get; set; }
        public bool IsActive { get; set; }
        public string ControllerName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int ParentLevel { get; set; }
        public int ItemLevel { get; set; }
        public string ItemTarget { get; set; } = string.Empty;
        public string IdName { get; set; } = string.Empty;
    }
    }
