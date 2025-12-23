using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using InventorySystem.Core.Models;
using InventorySystem.Data.Repositories;
using InventorySystem.UI.Views;

namespace InventorySystem.UI.Views {
    public partial class ProductsView : UserControl {
        private readonly ProductRepository _repository = new ProductRepository();

        // Pagination 
        private int _currentPage = 1;
        private int _pageSize = 10; // How many items per page
        private int _totalItems = 0;
        private int _totalPages = 0;

        public ProductsView() {
            InitializeComponent();
            LoadCategories();
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

        private void LoadCategories() {
            var categories = _repository.GetCategories();

            categories.Insert(0, "All Categories");

            CmbCategories.ItemsSource = categories;
            CmbCategories.SelectedIndex = 0;
        }

        private void LoadData() {
            if (ProductsGrid == null || CmbCategories == null || TxtSearch == null)
                return;
            string search = TxtSearch.Text == "Search products..." ? "" : TxtSearch.Text;
            string category = CmbCategories.SelectedValue as string ?? "";
            var result = _repository.GetProducts(search, category, _currentPage, _pageSize);

            _totalItems = result.TotalCount; // The count of filtered items
            ProductsGrid.ItemsSource = result.Products; // The 10 actual items

            // Recalculate Pages
            _totalPages = (int)Math.Ceiling((double)_totalItems / _pageSize);
            if (_totalPages == 0)
                _totalPages = 1;

            UpdatePaginationUI();
        }

        // When user types in search box
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) {
            // Always reset to Page 1 when searching.
            // If you are on Page 5 and search for "mac", "mac" might only have 1 page of results.
            _currentPage = 1;
            LoadData();
        }

        // When user changes category
        private void CmbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            _currentPage = 1;
            LoadData();
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e) {
            if (TxtSearch.Text == "Search products...") {
                TxtSearch.Text = "";
                TxtSearch.Foreground = Brushes.White;
            }
        }

        private void AddPlaceholder(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text)) {
                TxtSearch.Text = "Search products...";
                TxtSearch.Foreground = (Brush)new BrushConverter().ConvertFrom("#9CA3AF")!;
            }
        }

        private void UpdatePaginationUI() {
            TxtCurrentPage.Text = _currentPage.ToString();

            // Update - "Showing 1-10 of 50" text
            if (_totalItems == 0) {
                TxtPageInfo.Text = "No Products found";
            }
            else {
                int start = ((_currentPage - 1) * _pageSize) + 1;
                int end = Math.Min(start + _pageSize - 1, _totalItems);
                TxtPageInfo.Text = $"Showing {start}-{end} of {_totalItems}";
            }

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