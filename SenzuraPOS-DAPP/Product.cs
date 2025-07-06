namespace SenzuraPOS_DAPP
{
    public class Product
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public string ModelNumber { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public string DisplayName => $"{Brand} {Name} ({ModelNumber})";
    }
}