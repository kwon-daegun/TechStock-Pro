using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InventorySystem.Core.Models;
using InventorySystem.Data;
using InventorySystem.Services;

namespace InventorySystem.UI.Views.Windows {
    public partial class AddUserWindow : Window {
        public AddUserWindow() {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(TxtUsername.Text) || string.IsNullOrWhiteSpace(TxtPassword.Password)) {
                MessageBox.Show("Username and Password are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string role = (CmbRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Staff";

            var newUser = new User
            {
                Username = TxtUsername.Text,
                PasswordHash = PasswordHelper.HashPassword(TxtPassword.Password),
                Role = role
            };

            using (var context = new InventoryDbContext()) {
                if (context.Users.Any(u => u.Username == newUser.Username)) {
                    MessageBox.Show("Username already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                context.Users.Add(newUser);
                context.SaveChanges();
            }

            MessageBox.Show("User created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}