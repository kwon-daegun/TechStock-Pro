using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Core.Models {
    public class OrderItem {
        [Key]
        public int Id { get; set; }

        // Link to the Order
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        // Link to the Product
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        // Storing price here too, in case the product price changes later.
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public decimal Total => UnitPrice * Quantity;
    }
}