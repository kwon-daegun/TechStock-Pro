using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Core.Models {
    public class Order {
        [Key]
        public int Id { get; set; } // The Order ID (e.g., 1024)

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Navigation Property: An order has a list of items
        public virtual List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Helper for UI: Counts how many items are in the list
        public int ItemsCount => OrderItems?.Count ?? 0;
    }
}