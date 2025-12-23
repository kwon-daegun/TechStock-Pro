using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Core.Models {
    public class User {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        public string Role { get; set; } = "Staff";
    }
}
