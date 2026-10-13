namespace ERPLite.Web.Models.Orders
{
    /// <summary>
    /// Lightweight product lookup consumed by the order-builder script.
    /// Serialized to JSON (camelCase by default), so the script reads
    /// <c>id</c>, <c>name</c> and <c>price</c>.
    /// </summary>
    public class ProductOptionViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}
