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

namespace SenzuraPOS_DAPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void StartBilling_Click(object sender, RoutedEventArgs e)
        {
            BillingWindow billingWindow = new BillingWindow(_currentUser);
            billingWindow.Show();
        }

        private void ViewReports_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow reportWindow = new ReportWindow(_currentUser);
            reportWindow.Show();
        }

        private void ManageInventory_Click(object sender, RoutedEventArgs e)
        {
            InventoryManagementWindow inventoryWindow = new InventoryManagementWindow();
            inventoryWindow.Show();
        }
    }
}