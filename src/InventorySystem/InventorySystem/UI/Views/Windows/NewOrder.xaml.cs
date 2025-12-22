using System;
using System.Collections.ObjectModel; // Necessary for Live cart
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InventorySystem.Core.Models;
using InventorySystem.Data;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.UI.Views.Windows {
    public partial class NewOrderWindow : Window {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        // This collection automatically notifies the UI when items are added/removed
        private ObservableCollection<CartItemViewModel> _cartItems = new ObservableCollection<CartItemViewModel>();

        public NewOrderWindow() {
            InitializeComponent();
            LoadProducts();
            CartGrid.ItemsSource = _cartItems;
        }

        private void LoadProducts() {
            // Only load products are in-stock
            var products = _context.Products
                                   .AsNoTracking()
                                   .Where(p => p.Stock > 0)
                                   .ToList();

            CmbProductSelect.ItemsSource = products;
            CmbProductSelect.DisplayMemberPath = "Name";
            CmbProductSelect.SelectedValuePath = "Id";
            CmbProductSelect.SelectedIndex = 0;
        }

       private void BtnAddToCart_Click(object sender, RoutedEventArgs e) {
            if (CmbProductSelect.SelectedItem is Product selectedProduct) {
                // Check if item is already in cart
                var existingItem = _cartItems.FirstOrDefault(i => i.ProductId == selectedProduct.Id);

                if (existingItem != null) {
                    // Validation: Don't sell more than you have.
                    if (existingItem.Quantity < selectedProduct.Stock) {
                        existingItem.Quantity++;
                        CalculateTotals();
                    }
                    else {
                        MessageBox.Show("Stock limit reached for this product!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else {
                    _cartItems.Add(new CartItemViewModel
                    {
                        MaxStock = selectedProduct.Stock,
                        Quantity = 1,
                        ProductId = selectedProduct.Id,
                        ProductName = selectedProduct.Name,
                        UnitPrice = selectedProduct.Price
                    });
                    
                    CalculateTotals();
                }
            }
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e) {
            if (sender is Button btn && btn.DataContext is CartItemViewModel item) {
                _cartItems.Remove(item);
                CalculateTotals();
            }
        }

        private void CalculateTotals() {
            decimal total = _cartItems.Sum(i => i.Subtotal);
            TxtGrandTotal.Text = $"₹ {total:N2}";
        }

        // Update totals when typing quantity
        private void TxtQuantity_LostFocus(object sender, RoutedEventArgs e) {
            // When user types a number and clicks away, recalculate
            CalculateTotals();
        }

        // Checkout
        private void BtnCheckout_Click(object sender, RoutedEventArgs e) {
            if (_cartItems.Count == 0)
                return;

            // Use a Transaction to ensure Stock and Order save together or not at all
            using (var transaction = _context.Database.BeginTransaction()) {
                try {
                    // Create order
                    var newOrder = new Order
                    {
                        OrderDate = DateTime.Now,
                        TotalAmount = _cartItems.Sum(i => i.Subtotal)
                    };
                    _context.Orders.Add(newOrder);
                    _context.SaveChanges();

                    // Update stock
                    foreach (var cartItem in _cartItems) {
                        var orderItem = new OrderItem
                        {
                            OrderId = newOrder.Id,
                            ProductId = cartItem.ProductId,
                            Quantity = cartItem.Quantity,
                            UnitPrice = cartItem.UnitPrice
                        };
                        _context.OrderItems.Add(orderItem);

                        // Decrease stock in database
                        var productInDb = _context.Products.Find(cartItem.ProductId);
                        if (productInDb != null) {
                            // Double check stock one last time
                            if (productInDb.Stock < cartItem.Quantity) {
                                throw new Exception($"Not enough stock for {productInDb.Name}");
                            }
                            productInDb.Stock -= cartItem.Quantity;
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit(); // Commit transaction

                    MessageBox.Show("Order placed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex) {
                    transaction.Rollback(); // Undo everything if error
                    MessageBox.Show($"Error: {ex.Message}", "Transaction Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }

    // Helper class - represents a row in the shopping cart
    public class CartItemViewModel : System.ComponentModel.INotifyPropertyChanged {
        private int _quantity = 1;

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int MaxStock { get; set; }

        public int Quantity {
            get => _quantity;
            set {
                if (value < 1)
                    value = 1;

                if (value > MaxStock)
                    value = MaxStock;

                if (_quantity != value) {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(Subtotal)); // Update the Price when Qty changes
                }
            }
        }

        public decimal Subtotal => UnitPrice * Quantity;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
}