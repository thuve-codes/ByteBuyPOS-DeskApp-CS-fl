using System.Windows;
using System.Collections.ObjectModel;

namespace SenzuraPOS_DAPP
{
    public partial class ReportWindow : Window
    {
        private DatabaseHelper dbHelper;
        private User _currentUser;

        public ObservableCollection<SaleReportItem> ReportItems { get; set; } = new ObservableCollection<SaleReportItem>();

        public ReportWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            string connectionString = "Server=localhost;Port=3306;Database=pos_db;Uid=root;Pwd=root;"; // Replace with your MySQL connection string
            dbHelper = new DatabaseHelper(connectionString);
            ReportListView.ItemsSource = ReportItems;
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(YearTextBox.Text, out int year) && int.TryParse(MonthTextBox.Text, out int month))
            {
                ReportItems.Clear();
                foreach (var item in dbHelper.GetMonthlySalesReport(year, month))
                {
                    ReportItems.Add(item);
                }

                decimal monthlyTurnover = dbHelper.GetMonthlyTurnover(year, month);
                MonthlyTurnoverTextBlock.Text = $"Monthly Turnover: {monthlyTurnover:C}";
            }
            else
            {
                MessageBox.Show("Please enter valid year and month.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
