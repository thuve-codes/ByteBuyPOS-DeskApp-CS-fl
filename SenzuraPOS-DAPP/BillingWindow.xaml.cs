using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SenzuraPOS_DAPP
{
    public partial class BillingWindow : Window
    {
        public ObservableCollection<CartItem> Cart { get; set; } = new();
        public ObservableCollection<Product> Products { get; set; } = new();
        private DatabaseHelper dbHelper;
        private User _currentUser;

        public BillingWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

            string connectionString = "Server=localhost;Port=3306;Database=pos_db;Uid=root;Pwd=root;"; // Replace with your MySQL connection string
            dbHelper = new DatabaseHelper(connectionString);

            LoadProducts();

            ProductComboBox.ItemsSource = Products;
            CartListView.ItemsSource = Cart;
        }

        private void LoadProducts()
        {
            Products.Clear();
            foreach (var product in dbHelper.GetProducts())
            {
                Products.Add(product);
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem is Product selectedProduct &&
                int.TryParse(QuantityTextBox.Text, out int quantity) &&
                quantity > 0)
            {
                // Fetch the latest stock quantity from the database
                var latestProduct = dbHelper.GetProducts().FirstOrDefault(p => p.Id == selectedProduct.Id);
                if (latestProduct == null)
                {
                    MessageBox.Show("Product not found in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (quantity > latestProduct.StockQuantity)
                {
                    MessageBox.Show($"Only {latestProduct.StockQuantity} units available in stock.", "Stock Limit", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var existingItem = Cart.FirstOrDefault(i => i.ModelNumber == selectedProduct.ModelNumber);
                if (existingItem != null)
                {
                    if (existingItem.Quantity + quantity > latestProduct.StockQuantity)
                    {
                        MessageBox.Show($"Adding {quantity} exceeds available stock.", "Stock Limit", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    existingItem.Quantity += quantity;
                    existingItem.Total = existingItem.Quantity * existingItem.Price;
                }
                else
                {
                    Cart.Add(new CartItem
                    {
                        Id = selectedProduct.Id,
                        Name = selectedProduct.Name,
                        Brand = selectedProduct.Brand,
                        ModelNumber = selectedProduct.ModelNumber,
                        Quantity = quantity,
                        Price = selectedProduct.Price,
                        Total = selectedProduct.Price * quantity
                    });
                }

                UpdateTotal();
                QuantityTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Please select a product and enter a valid quantity.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateTotal()
        {
            decimal total = Cart.Sum(item => item.Total);
            TotalAmountTextBlock.Text = $"Rs. {total:N2}";
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            if (Cart.Count == 0)
            {
                MessageBox.Show("Cart is empty. Please add items to checkout.", "Checkout", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Deduct stock quantity after checkout
            foreach (var item in Cart)
            {
                dbHelper.UpdateProductStock(item.Id, -item.Quantity);
                dbHelper.InsertSale(item.Id, item.Quantity, item.Price, item.Total, _currentUser.Id);
            }

            decimal totalAmount = Cart.Sum(item => item.Total);
            PdfGenerator pdfGenerator = new PdfGenerator();
            pdfGenerator.GenerateInvoice(Cart, totalAmount);

            MessageBox.Show($"Checkout complete! Total amount: {TotalAmountTextBlock.Text}", "Checkout", MessageBoxButton.OK, MessageBoxImage.Information);

            Cart.Clear();
            UpdateTotal();
            LoadProducts(); // Reload products to reflect updated stock
        }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public string ModelNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
