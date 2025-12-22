using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InventorySystem.Core.Models;

namespace InventorySystem.UI.Views.Windows {
    /// <summary>
    /// Interaction logic for OrderDetailsWindow.xaml
    /// </summary>
    public partial class OrderDetailsWindow : Window {
        public OrderDetailsWindow(Order order) {
            InitializeComponent();

            // Populate UI
            TxtOrderId.Text = $"Order #{order.Id:D5}";
            TxtOrderDate.Text = order.OrderDate.ToString("MMMM dd, yyyy - hh:mm tt");
            TxtTotalAmount.Text = $"₹ {order.TotalAmount:N2}";

            // Populate Grid
            DetailsGrid.ItemsSource = order.OrderItems;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
