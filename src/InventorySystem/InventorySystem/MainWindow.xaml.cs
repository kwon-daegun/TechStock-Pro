using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InventorySystem.Services;
using InventorySystem.UI.Views;

namespace InventorySystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ActiveView.Content = new ProductsView();
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e) {
            ActiveView.Content = new ComingSoonView();
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e) {
            ActiveView.Content = new ProductsView();
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e) {
            ActiveView.Content = new OrdersView();
        }

        private void BtnSuppliers_Click(object sender, RoutedEventArgs e) {
            ActiveView.Content = new SuppliersView();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e) {
            ActiveView.Content = new ComingSoonView();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e) {
            ActiveView.Content = new SettingsView();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e) {
            LoginView login = new LoginView();
            login.Show();
            this.Close();
        }
    }
}