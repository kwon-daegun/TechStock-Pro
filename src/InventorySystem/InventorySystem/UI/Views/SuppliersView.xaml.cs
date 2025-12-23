using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InventorySystem.Core; // For Session
using InventorySystem.Core.Models;
using InventorySystem.Data;
using InventorySystem.Services;
using InventorySystem.UI.Views.Windows; // For SupplierFormWindow
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.UI.Views {
    public partial class SuppliersView : UserControl {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        // Pagination State
        private int _currentPage = 1;
        private int _pageSize = 10; // Suppliers rows are taller, so maybe 10 per page
        private int _totalItems = 0;
        private int _totalPages = 0;

        public SuppliersView() {
            InitializeComponent();
            LoadData();
        }

        private void LoadData() {
            // Safety Check
            if (SuppliersGrid == null || TxtSearch == null)
                return;

            string search = TxtSearch.Text == "Search suppliers..." ? "" : TxtSearch.Text.ToLower();

            var query = _context.Suppliers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search)) {
                query = query.Where(s => s.Name.ToLower().Contains(search) ||
                                         s.ContactPerson.ToLower().Contains(search) ||
                                         s.Email.ToLower().Contains(search));
            }

            _totalItems = query.Count();

            _totalPages = (int)Math.Ceiling((double)_totalItems / _pageSize);
            if (_totalPages == 0)
                _totalPages = 1;

            if (_currentPage > _totalPages)
                _currentPage = _totalPages;

            var suppliers = query.OrderByDescending(s => s.Id)
                                 .Skip((_currentPage - 1) * _pageSize)
                                 .Take(_pageSize)
                                 .ToList();

            SuppliersGrid.ItemsSource = suppliers;

            UpdatePaginationUI();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) {
            _currentPage = 1;
            LoadData();
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e) {
            if (TxtSearch.Text == "Search suppliers...") {
                TxtSearch.Text = "";
                TxtSearch.Foreground = Brushes.White;
            }
        }

        private void AddPlaceholder(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text)) {
                TxtSearch.Text = "Search suppliers...";
                TxtSearch.Foreground = (Brush)new BrushConverter().ConvertFrom("#9CA3AF")!; // Gray
            }
        }

        private void BtnAddSupplier_Click(object sender, RoutedEventArgs e) {
            var form = new SupplierFormWindow();
            form.ShowDialog();
            LoadData();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e) {
            if (sender is Button btn && btn.DataContext is Supplier selectedSupplier) {
                var form = new SupplierFormWindow(selectedSupplier);
                form.ShowDialog();
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e) {
            // Security Check
            if (!Session.IsAdmin()) {
                MessageBox.Show("Only Administrators can delete suppliers.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (sender is Button btn && btn.DataContext is Supplier supplier) {
                var result = MessageBox.Show($"Are you sure you want to delete {supplier.Name}?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes) {
                    try {
                        var dbSupplier = _context.Suppliers.Find(supplier.Id);
                        if (dbSupplier != null) {
                            _context.Suppliers.Remove(dbSupplier);
                            _context.SaveChanges();
                            LoadData();
                            MessageBox.Show("Supplier deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex) {
                        MessageBox.Show($"Error deleting supplier: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void UpdatePaginationUI() {
            TxtCurrentPage.Text = _currentPage.ToString();

            if (_totalItems == 0) {
                TxtPageInfo.Text = "No suppliers found";
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