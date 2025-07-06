using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SenzuraPOS_DAPP
{
    public partial class BillingWindow : Window
    {
        public ObservableCollection<CartItem> Cart { get; set; } = new();
        public List<Product> Products { get; set; } = new();

        public BillingWindow()
        {
            InitializeComponent();

            // Sample Products
            Products.Add(new Product { Name = "Milk", Price = 80 });
            Products.Add(new Product { Name = "Bread", Price = 50 });
            Products.Add(new Product { Name = "Eggs", Price = 15 });

            ProductComboBox.ItemsSource = Products;
            ProductComboBox.DisplayMemberPath = "Name";

            CartListView.ItemsSource = Cart;
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem is Product selectedProduct && int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                var item = new CartItem
                {
                    Name = selectedProduct.Name,
                    Quantity = quantity,
                    Price = selectedProduct.Price,
                    Total = selectedProduct.Price * quantity
                };

                Cart.Add(item);
                UpdateTotal();
                QuantityTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Please select a product and enter a valid quantity.");
            }
        }

        private void UpdateTotal()
        {
            decimal total = Cart.Sum(item => item.Total);
            TotalAmountTextBlock.Text = $"Rs. {total:N2}";
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Checkout complete! Thank you.");
            Cart.Clear();
            UpdateTotal();
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class CartItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
