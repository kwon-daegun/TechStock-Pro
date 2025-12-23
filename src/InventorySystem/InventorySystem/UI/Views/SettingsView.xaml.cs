using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InventorySystem.Core; // For Session
using InventorySystem.Core.Models;
using InventorySystem.Data;
using InventorySystem.Services;
using InventorySystem.UI.Views.Windows; // For AddUserWindow

namespace InventorySystem.UI.Views {
    public partial class SettingsView : UserControl {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        public SettingsView() {
            InitializeComponent();
            CheckPermissions();
        }

        private void CheckPermissions() {
           if (!Session.IsAdmin()) {
                TabUsers.Visibility = Visibility.Collapsed;
                BtnAddUser.Visibility = Visibility.Collapsed;
            }
            else {
                LoadUsers();
            }
        }

        private void BtnUpdatePassword_Click(object sender, RoutedEventArgs e) {
            var currentUser = Session.CurrentUser;
            if (currentUser == null)
                return;

            string oldPass = TxtOldPass.Password;
            string newPass = TxtNewPass.Password;
            string confirmPass = TxtConfirmPass.Password;

            if (string.IsNullOrWhiteSpace(oldPass) || string.IsNullOrWhiteSpace(newPass)) {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPass != confirmPass) {
                MessageBox.Show("New passwords do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!PasswordHelper.VerifyPassword(oldPass, currentUser.PasswordHash)) {
                MessageBox.Show("Incorrect current password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var userInDb = _context.Users.Find(currentUser.Id);
            if (userInDb != null) {
                userInDb.PasswordHash = PasswordHelper.HashPassword(newPass);
                _context.SaveChanges();

                // Update Session
                currentUser.PasswordHash = userInDb.PasswordHash;

                MessageBox.Show("Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                TxtOldPass.Password = "";
                TxtNewPass.Password = "";
                TxtConfirmPass.Password = "";

                LoadUsers();
            }
        }

        // user management
        private void LoadUsers() {
            if (Session.IsAdmin()) {
                UsersGrid.ItemsSource = _context.Users.ToList();
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e) {
            var win = new AddUserWindow();
            win.ShowDialog();
            LoadUsers();
        }

        private void BtnResetPass_Click(object sender, RoutedEventArgs e) {
            if (sender is Button btn && btn.DataContext is User selectedUser) {
                var result = MessageBox.Show($"Reset password for '{selectedUser.Username}' to '1234'?",
                                             "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes) {
                    var dbUser = _context.Users.Find(selectedUser.Id);
                    if (dbUser != null) {
                        dbUser.PasswordHash = PasswordHelper.HashPassword("1234");
                        _context.SaveChanges();
                        MessageBox.Show($"Password for {selectedUser.Username} is now '1234'.", "Success");
                    }
                }
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e) {
            // If for some reason session is lost, stop here.
            if (Session.CurrentUser == null)
                return;

            if (sender is Button btn && btn.DataContext is User selectedUser) {
                if (selectedUser.Id == Session.CurrentUser.Id) {
                    MessageBox.Show("You cannot delete your own account while logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show($"Delete user '{selectedUser.Username}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                    var dbUser = _context.Users.Find(selectedUser.Id);
                    if (dbUser != null) {
                        _context.Users.Remove(dbUser);
                        _context.SaveChanges();
                        LoadUsers();
                    }
                }
            }
        }
    }
}