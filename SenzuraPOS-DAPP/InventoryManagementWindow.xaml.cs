using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;

namespace SenzuraPOS_DAPP
{
    public partial class InventoryManagementWindow : Window
    {
        private DatabaseHelper dbHelper;
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        public InventoryManagementWindow()
        {
            InitializeComponent();
            string connectionString = "Server=localhost;Port=3306;Database=pos_db;Uid=root;Pwd=root;"; // Replace with your MySQL connection string
            dbHelper = new DatabaseHelper(connectionString);
            ProductListView.ItemsSource = Products;
            LoadProducts();
        }

        private void LoadProducts()
        {
            Products.Clear();
            foreach (var product in dbHelper.GetProducts())
            {
                Products.Add(product);
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(PriceTextBox.Text, out decimal price) && int.TryParse(StockQuantityTextBox.Text, out int stockQuantity))
            {
                Product newProduct = new Product
                {
                    Brand = BrandTextBox.Text,
                    Name = NameTextBox.Text,
                    ModelNumber = ModelNumberTextBox.Text,
                    Price = price,
                    StockQuantity = stockQuantity
                };
                dbHelper.AddProduct(newProduct);
                LoadProducts();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Please enter valid price and stock quantity.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedItem is Product selectedProduct &&
                decimal.TryParse(PriceTextBox.Text, out decimal price) &&
                int.TryParse(StockQuantityTextBox.Text, out int stockQuantity))
            {
                selectedProduct.Brand = BrandTextBox.Text;
                selectedProduct.Name = NameTextBox.Text;
                selectedProduct.ModelNumber = ModelNumberTextBox.Text;
                selectedProduct.Price = price;
                selectedProduct.StockQuantity = stockQuantity;

                dbHelper.UpdateProduct(selectedProduct);
                LoadProducts();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Please select a product and enter valid data.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedItem is Product selectedProduct)
            {
                dbHelper.DeleteProduct(selectedProduct.Id);
                LoadProducts();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Please select a product to delete.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ProductListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProductListView.SelectedItem is Product selectedProduct)
            {
                ProductIdTextBox.Text = selectedProduct.Id.ToString();
                BrandTextBox.Text = selectedProduct.Brand;
                NameTextBox.Text = selectedProduct.Name;
                ModelNumberTextBox.Text = selectedProduct.ModelNumber;
                PriceTextBox.Text = selectedProduct.Price.ToString();
                StockQuantityTextBox.Text = selectedProduct.StockQuantity.ToString();
            }
            else
            {
                ClearForm();
            }
        }

        private void ClearForm()
        {
            ProductIdTextBox.Clear();
            BrandTextBox.Clear();
            NameTextBox.Clear();
            ModelNumberTextBox.Clear();
            PriceTextBox.Clear();
            StockQuantityTextBox.Clear();
        }
    }
}
