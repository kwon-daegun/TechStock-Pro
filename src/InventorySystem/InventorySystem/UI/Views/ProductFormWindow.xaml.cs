using System;
using System.Windows;
using InventorySystem.Core.Models;
using InventorySystem.Data.Repositories;

namespace InventorySystem.UI.Views {
    public partial class ProductFormWindow : Window {
        private readonly ProductRepository _repository = new ProductRepository();
        private Product? _productToEdit = null; // Stores product if we are editing

        public ProductFormWindow() {
            InitializeComponent();
        }

        // Edit Mode (Takes a product as input)
        public ProductFormWindow(Product product) {
            InitializeComponent();
            _productToEdit = product;

            // Pre-fill the fields with existing data
            TxtName.Text = product.Name;
            TxtBrand.Text = product.Brand;
            TxtCategory.Text = product.Category;
            TxtPrice.Text = product.Price.ToString("N2");
            TxtStock.Text = product.Stock.ToString();
            TxtImage.Text = product.ImageUrl;

            // Show the Delete button (only visible when editing)
            BtnDelete.Visibility = Visibility.Visible;
            Title = "Edit Product";
        }

        // Save button logic
        private void BtnSave_Click(object sender, RoutedEventArgs e) {
            try {
                if (string.IsNullOrWhiteSpace(TxtName.Text) || string.IsNullOrWhiteSpace(TxtPrice.Text)) {
                    MessageBox.Show("Name and Price are required!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(TxtPrice.Text, out decimal price))
                    price = 0;
                if (!int.TryParse(TxtStock.Text, out int stock))
                    stock = 0;

                if (_productToEdit == null) {
                    _productToEdit = new Product
                    {
                        Name = TxtName.Text,
                        Price = price,
                        Category = TxtCategory.Text,
                        Brand = TxtBrand.Text,
                        Stock = stock,
                        ImageUrl = TxtImage.Text
                    };
                }
                else {
                    _productToEdit.Name = TxtName.Text;
                    _productToEdit.Brand = TxtBrand.Text;
                    _productToEdit.Category = TxtCategory.Text;
                    _productToEdit.Price = price;
                    _productToEdit.Stock = stock;
                    _productToEdit.ImageUrl = TxtImage.Text;
                }

                // Save to Database
                if (_productToEdit.Id == 0) {
                    _repository.Add(_productToEdit);
                }
                else {
                    _repository.Update(_productToEdit);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex) {
                MessageBox.Show($"Error saving: {ex.Message}");
            }
        }

        // Delete button logic
        private void BtnDelete_Click(object sender, RoutedEventArgs e) {
            if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                _repository.Delete(_productToEdit.Id);
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}