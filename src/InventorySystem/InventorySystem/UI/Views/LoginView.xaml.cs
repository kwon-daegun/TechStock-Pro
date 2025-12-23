using System.Windows;
using System.Linq;
using InventorySystem.Data;
using InventorySystem.Services;

namespace InventorySystem
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            using (var context = new InventoryDbContext()) {
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user != null && PasswordHelper.VerifyPassword(password, user.PasswordHash)) {
                    Session.Start(user);

                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
                }
                else {
                    MessageBox.Show("Invalid username or password.");
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}