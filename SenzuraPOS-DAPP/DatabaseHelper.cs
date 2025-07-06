using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Windows;

namespace SenzuraPOS_DAPP
{
    public class DatabaseHelper
    {
        private readonly string connectionString;

        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public ObservableCollection<Product> GetProducts()
        {
            ObservableCollection<Product> products = new ObservableCollection<Product>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Id, Brand, Name, ModelNumber, Price, StockQuantity FROM product";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                Id = reader.GetInt32("Id"),
                                Brand = reader.GetString("Brand"),
                                Name = reader.GetString("Name"),
                                ModelNumber = reader.GetString("ModelNumber"),
                                Price = reader.GetDecimal("Price"),
                                StockQuantity = reader.GetInt32("StockQuantity")
                            });
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
            return products;
        }

        public void UpdateProductStock(int productId, int quantityChange)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE product SET StockQuantity = StockQuantity + @quantityChange WHERE Id = @productId";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@quantityChange", quantityChange);
                    command.Parameters.AddWithValue("@productId", productId);
                    command.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Database error updating stock: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public User Login(string username, string password)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Id, Username, PasswordHash, Role FROM users WHERE Username = @username AND PasswordHash = @password"; // In a real app, use proper password hashing and salting
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password); // This should be a hashed password

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("Id"),
                                Username = reader.GetString("Username"),
                                PasswordHash = reader.GetString("PasswordHash"),
                                Role = reader.GetString("Role")
                            };
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Database error during login: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
            return null;
        }

        public void InsertSale(int productId, int quantity, decimal price, decimal total, int userId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO sales (ProductId, Quantity, Price, Total, SaleDate, UserId) VALUES (@productId, @quantity, @price, @total, @saleDate, @userId)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@productId", productId);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.Parameters.AddWithValue("@price", price);
                    command.Parameters.AddWithValue("@total", total);
                    command.Parameters.AddWithValue("@saleDate", System.DateTime.Now);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Database error inserting sale: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public ObservableCollection<SaleReportItem> GetMonthlySalesReport(int year, int month)
        {
            ObservableCollection<SaleReportItem> reportItems = new ObservableCollection<SaleReportItem>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT p.Name, p.Brand, SUM(s.Quantity) as TotalQuantity, SUM(s.Total) as TotalRevenue FROM sales s JOIN product p ON s.ProductId = p.Id WHERE YEAR(s.SaleDate) = @year AND MONTH(s.SaleDate) = @month GROUP BY p.Name, p.Brand";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@year", year);
                    command.Parameters.AddWithValue("@month", month);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reportItems.Add(new SaleReportItem
                            {
                                ProductName = reader.GetString("Name"),
                                ProductBrand = reader.GetString("Brand"),
                                TotalQuantitySold = reader.GetInt32("TotalQuantity"),
                                TotalRevenue = reader.GetDecimal("TotalRevenue")
                            });
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Database error generating report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
            return reportItems;
        }

        public decimal GetMonthlyTurnover(int year, int month)
        {
            decimal totalTurnover = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT SUM(Total) FROM sales WHERE YEAR(SaleDate) = @year AND MONTH(SaleDate) = @month";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@year", year);
                    command.Parameters.AddWithValue("@month", month);

                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        totalTurnover = Convert.ToDecimal(result);
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Database error calculating monthly turnover: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
            return totalTurnover;
        }

        public void AddProduct(Product product)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO product (Brand, Name, ModelNumber, Price, StockQuantity) VALUES (@brand, @name, @modelNumber, @price, @stockQuantity)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@brand", product.Brand);
                    command.Parameters.AddWithValue("@name", product.Name);
                    command.Parameters.AddWithValue("@modelNumber", product.ModelNumber);
                    command.Parameters.AddWithValue("@price", product.Price);
                    command.Parameters.AddWithValue("@stockQuantity", product.StockQuantity);
                    command.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Database error adding product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void UpdateProduct(Product product)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE product SET Brand = @brand, Name = @name, ModelNumber = @modelNumber, Price = @price, StockQuantity = @stockQuantity WHERE Id = @id";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@brand", product.Brand);
                    command.Parameters.AddWithValue("@name", product.Name);
                    command.Parameters.AddWithValue("@modelNumber", product.ModelNumber);
                    command.Parameters.AddWithValue("@price", product.Price);
                    command.Parameters.AddWithValue("@stockQuantity", product.StockQuantity);
                    command.Parameters.AddWithValue("@id", product.Id);
                    command.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Database error updating product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void DeleteProduct(int productId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM product WHERE Id = @productId";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@productId", productId);
                    command.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Database error deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
