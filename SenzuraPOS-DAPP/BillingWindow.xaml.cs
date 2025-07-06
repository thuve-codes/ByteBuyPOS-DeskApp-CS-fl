using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SenzuraPOS_DAPP
{
    public partial class BillingWindow : Window
    {
        public ObservableCollection<CartItem> Cart { get; set; } = new();
        public ObservableCollection<Product> Products { get; set; } = new();

        public BillingWindow()
        {
            InitializeComponent();

            // Sample electronics products with brand/model/stock
            Products.Add(new Product { Brand = "Sony", Name = "4K TV", ModelNumber = "X900H", Price = 1200m, StockQuantity = 10 });
            Products.Add(new Product { Brand = "Apple", Name = "iPhone", ModelNumber = "13 Pro", Price = 999m, StockQuantity = 15 });
            Products.Add(new Product { Brand = "Dell", Name = "Laptop", ModelNumber = "XPS 15", Price = 1500m, StockQuantity = 8 });

            ProductComboBox.ItemsSource = Products;
            CartListView.ItemsSource = Cart;
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem is Product selectedProduct &&
                int.TryParse(QuantityTextBox.Text, out int quantity) &&
                quantity > 0)
            {
                if (quantity > selectedProduct.StockQuantity)
                {
                    MessageBox.Show($"Only {selectedProduct.StockQuantity} units available in stock.", "Stock Limit", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var existingItem = Cart.FirstOrDefault(i => i.ModelNumber == selectedProduct.ModelNumber);
                if (existingItem != null)
                {
                    if (existingItem.Quantity + quantity > selectedProduct.StockQuantity)
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
                var product = Products.FirstOrDefault(p => p.ModelNumber == item.ModelNumber);
                if (product != null)
                    product.StockQuantity -= item.Quantity;
            }

            MessageBox.Show($"Checkout complete! Total amount: {TotalAmountTextBlock.Text}", "Checkout", MessageBoxButton.OK, MessageBoxImage.Information);

            Cart.Clear();
            UpdateTotal();
        }
    }

    public class Product
    {
        public string Brand { get; set; }
        public string Name { get; set; }
        public string ModelNumber { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public string DisplayName => $"{Brand} {Name} ({ModelNumber})";
    }

    public class CartItem
    {
        public string Brand { get; set; }
        public string Name { get; set; }
        public string ModelNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
