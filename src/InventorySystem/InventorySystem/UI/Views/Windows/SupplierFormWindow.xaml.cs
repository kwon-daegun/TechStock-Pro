using System.Windows;
using InventorySystem.Core.Models;
using InventorySystem.Data;

namespace InventorySystem.UI.Views.Windows {
    public partial class SupplierFormWindow : Window {
        private readonly Supplier _supplier;
        private readonly bool _isEditMode;

        public SupplierFormWindow() {
            InitializeComponent();
            _supplier = new Supplier { Name = string.Empty };
            _isEditMode = false;
        }

        public SupplierFormWindow(Supplier supplierToEdit) {
            InitializeComponent();
            _supplier = supplierToEdit;
            _isEditMode = true;

            // Fill the form with existing data
            TxtTitle.Text = "Edit Supplier";
            InputName.Text = _supplier.Name;
            InputContact.Text = _supplier.ContactPerson;
            InputEmail.Text = _supplier.Email;
            InputPhone.Text = _supplier.Phone;
            InputAddress.Text = _supplier.Address;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(InputName.Text)) {
                MessageBox.Show("Company Name is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _supplier.Name = InputName.Text;
            _supplier.ContactPerson = InputContact.Text;
            _supplier.Email = InputEmail.Text;
            _supplier.Phone = InputPhone.Text;
            _supplier.Address = InputAddress.Text;

            using (var context = new InventoryDbContext()) {
                if (_isEditMode) {
                    context.Suppliers.Update(_supplier);
                }
                else {
                    context.Suppliers.Add(_supplier);
                }
                context.SaveChanges();
            }

            MessageBox.Show("Supplier saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}