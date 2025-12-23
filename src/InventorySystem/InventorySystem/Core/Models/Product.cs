using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Core.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; } // "Infinix Smart 6"

        [Required]
        [Column(TypeName = "decimal(18,2)")] // for precision
        public required decimal Price { get; set; }

        [Required]
        public required string Category { get; set; } // "Mobiles", "Laptops"

        public string Brand { get; set; } = "Generic"; // "Infinix", "Realme"
        public int Stock { get; set; } = 0;
        public string ImageUrl { get; set; } = "../../UI/Assets/Images/Icons/placholder_icon.png";

        public string StatusText {
            get {
                if (Stock == 0) return "Out Of Stock";
                if (Stock <= 10) return "Low Stock";
                return "In Stock";
            }
        }

        public string StatusTextClr {
            get {
                if (Stock == 0)
                    return "#EF4444";
                if (Stock <= 10)
                    return "#F59E0B";
                return "#10B981";
            }
        }
        public string StatusBgClr {
            get {
                if (Stock == 0)
                    return "#451a1a";
                if (Stock <= 10)
                    return "#451a03";
                return "#064e3b";
            }
        }

        public string StockLevelClr => StatusTextClr;
    }
}
