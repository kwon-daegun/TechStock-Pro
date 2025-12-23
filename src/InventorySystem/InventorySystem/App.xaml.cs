using System.Configuration;
using System.Linq;
using System.Windows;
using InventorySystem.Data;        // Need this to see the Database
using InventorySystem.Core.Models; // Need this to see 'User'
using InventorySystem.Services;    // Need this to see 'PasswordHelper'

namespace InventorySystem {
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            // Connect to database
            using (var context = new InventoryDbContext()) {
                context.Database.EnsureCreated();

                // Check if empty
                if (!context.Users.Any()) {
                    var admin = new User
                    {
                        Username = "admin",
                        PasswordHash = PasswordHelper.HashPassword("1234"),
                        Role = "Admin"
                    };

                    context.Users.Add(admin);
                    context.SaveChanges();
                }

                DatabaseSeeder.Seed(context);
            }

            var loginWindow = new LoginView();
            loginWindow.Show();
        }
    }
}