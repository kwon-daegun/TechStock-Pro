using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InventorySystem.Core; // For Session
using InventorySystem.Core.Models;
using InventorySystem.Data;
using InventorySystem.Data.Repositories;
using InventorySystem.Services;
using InventorySystem.UI.Views.Windows;
using Microsoft.EntityFrameworkCore; // For .Include()


namespace InventorySystem.UI.Views {
    public partial class OrdersView : UserControl {
        private readonly OrderRepository _repository = new OrderRepository();

        // Pagination State
        private int _currentPage = 1;
        private int _pageSize = 12; // How many orders per page
        private int _totalItems = 0;
        private int _totalPages = 0;

        public OrdersView() {
            InitializeComponent();

            if (!Session.IsAdmin()) {
                ColDelete.Visibility = Visibility.Collapsed;
            }
            LoadData();
        }

        private void LoadData() {
            // Safety Check
            if (OrdersGrid == null || TxtSearchOrder == null || CmbDateFilter == null)
                return;

            // Get Inputs
            string search = TxtSearchOrder.Text == "Search Order ID..." ? "" : TxtSearchOrder.Text;

            // Get Date Filter safely
            string dateFilter = "All Time";
            if (CmbDateFilter.SelectedItem is ComboBoxItem item) {
                dateFilter = item.Content?.ToString() ?? "All Time";
            }

            // Call Repository
            var result = _repository.GetOrders(search, dateFilter, _currentPage, _pageSize);

            _totalItems = result.TotalCount;
            OrdersGrid.ItemsSource = result.Orders;

            // Update Pagination UI
            _totalPages = (int)Math.Ceiling((double)_totalItems / _pageSize);
            if (_totalPages == 0)
                _totalPages = 1;

            UpdatePaginationUI();
        }

        private void TxtSearchOrder_TextChanged(object sender, TextChangedEventArgs e) {
            _currentPage = 1;
            LoadData();
        }

        private void CmbDateFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            _currentPage = 1;
            LoadData();
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e) {
            if (TxtSearchOrder.Text == "Search Order ID...") {
                TxtSearchOrder.Text = "";
                TxtSearchOrder.Foreground = Brushes.White;
            }
        }

        private void AddPlaceholder(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(TxtSearchOrder.Text)) {
                TxtSearchOrder.Text = "Search Order ID...";
                TxtSearchOrder.Foreground = Brushes.Gray;
            }
        }

        private void BtnNewOrder_Click(object sender, RoutedEventArgs e) {
            var newOrderWindow = new InventorySystem.UI.Views.Windows.NewOrderWindow();
            newOrderWindow.ShowDialog();

            // Refresh the list after they close the window - to see the new order
            LoadData();
        }

        private void BtnViewOrder_Click(object sender, RoutedEventArgs e) {
            if (sender is Button btn && btn.DataContext is Order selectedOrder) {
                var detailsWin = new OrderDetailsWindow(selectedOrder);
                detailsWin.ShowDialog();
            }
        }

        private void BtnDeleteOrder_Click(object sender, RoutedEventArgs e) {
            // Security - Only Admins can delete orders
            if (!Session.IsAdmin()) {
                MessageBox.Show("Only Administrators can delete orders.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get order
            if (sender is Button btn && btn.DataContext is Order selectedOrderRow) {
                // Confirm deletion
                var result = MessageBox.Show($"Are you sure you want to delete Order #{selectedOrderRow.Id:D5}?\n\nThis will restore the stock for all items in this order.",
                                             "Confirm Deletion",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes) {
                    DeleteOrder(selectedOrderRow.Id);
                }
            }
        }

        private void DeleteOrder(int orderId) {
            using (var context = new InventoryDbContext()) {
                using (var transaction = context.Database.BeginTransaction()) {
                    try {
                        // Find order and it's items
                        var orderInDb = context.Orders
                                               .Include(o => o.OrderItems)
                                               .FirstOrDefault(o => o.Id == orderId);

                        if (orderInDb == null)
                            return;

                        // restore stock
                        foreach (var item in orderInDb.OrderItems) {
                            var product = context.Products.Find(item.ProductId);
                            if (product != null) {
                                product.Stock += item.Quantity; // Put items back
                            }
                        }

                        // delete order
                        context.Orders.Remove(orderInDb);
                        context.SaveChanges();
                        transaction.Commit();

                        LoadData();
                        MessageBox.Show("Order deleted and stock restored.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (System.Exception ex) {
                        transaction.Rollback();
                        MessageBox.Show($"Error deleting order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void UpdatePaginationUI() {
            TxtCurrentPage.Text = _currentPage.ToString();

            if (_totalItems == 0) {
                TxtPageInfo.Text = "No orders found";
            }
            else {
                int start = ((_currentPage - 1) * _pageSize) + 1;
                int end = Math.Min(start + _pageSize - 1, _totalItems);
                TxtPageInfo.Text = $"Showing {start}-{end} of {_totalItems}";
            }

            BtnPrevious.IsEnabled = _currentPage > 1;
            BtnNext.IsEnabled = _currentPage < _totalPages;

            BtnPrevious.Opacity = _currentPage > 1 ? 1.0 : 0.3;
            BtnNext.Opacity = _currentPage < _totalPages ? 1.0 : 0.3;
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e) {
            if (_currentPage > 1) {
                _currentPage--;
                LoadData();
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e) {
            if (_currentPage < _totalPages) {
                _currentPage++;
                LoadData();
            }
        }
    }
}