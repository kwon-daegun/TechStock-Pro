using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InventorySystem.Core.Models;
using InventorySystem.Data.Repositories;
using InventorySystem.UI.Views;

namespace InventorySystem.UI.Views {
    public partial class ProductsView : UserControl {
        private readonly ProductRepository _repository = new ProductRepository();

        public ProductsView() {
            InitializeComponent();
            LoadData();
        }

        // Add products button click
        private void BtnAdd_Click(object sender, RoutedEventArgs e) {
            var form = new ProductFormWindow();

            // ShowDialog pauses this code until the popup closes
            if (form.ShowDialog() == true) {
                LoadData();
            }
        }

        // Edit products by double clicking
        private void ProductsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (ProductsGrid.SelectedItem is Product selectedProduct) {
                var form = new ProductFormWindow(selectedProduct);

                if (form.ShowDialog() == true) {
                    LoadData();
                }
            }
        }

        // Pagination 
        private int _currentPage = 1;
        private int _pageSize = 10; // How many items per page
        private int _totalItems = 0;
        private int _totalPages = 0;

        private void LoadData() {
            _totalItems = _repository.GetTotalCount();
            _totalPages = (int)Math.Ceiling((double)_totalItems / _pageSize);

            if (_totalPages == 0)
                _totalPages = 1; // Safety for empty DB

            var products = _repository.GetProductsByPage(_currentPage, _pageSize);
            ProductsGrid.ItemsSource = products;

            UpdatePaginationUI();
        }

        private void UpdatePaginationUI() {
            TxtCurrentPage.Text = _currentPage.ToString();

            // Update - "Showing 1-10 of 50" text
            int start = ((_currentPage - 1) * _pageSize) + 1;
            int end = Math.Min(start + _pageSize - 1, _totalItems);
            TxtPageInfo.Text = $"Showing {start}-{end} of {_totalItems}";

            // Enable/Disable Buttons based on where we are
            BtnPrevious.IsEnabled = _currentPage > 1;
            BtnNext.IsEnabled = _currentPage < _totalPages;

            // Dim the buttons - if disabled
            BtnPrevious.Opacity = _currentPage > 1 ? 1.0 : 0.3;
            BtnNext.Opacity = _currentPage < _totalPages ? 1.0 : 0.3;
        }

        // Button clicks
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