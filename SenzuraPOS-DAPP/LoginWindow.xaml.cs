using System.Windows;

namespace SenzuraPOS_DAPP
{
    public partial class LoginWindow : Window
    {
        private DatabaseHelper dbHelper;

        public LoginWindow()
        {
            InitializeComponent();
            string connectionString = "Server=localhost;Port=3306;Database=pos_db;Uid=root;Pwd=root;"; // Replace with your MySQL connection string
            dbHelper = new DatabaseHelper(connectionString);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            User user = dbHelper.Login(username, password);

            if (user != null)
            {
                MessageBox.Show($"Welcome, {user.Username}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow mainWindow = new MainWindow(user);
                Application.Current.MainWindow = mainWindow; // Set the new main window
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}