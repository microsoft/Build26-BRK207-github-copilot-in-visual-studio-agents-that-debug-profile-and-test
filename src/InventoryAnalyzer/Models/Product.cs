namespace InventoryAnalyzer.Models;

public class Product
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int CategoryId { get; set; }
    public int UnitsSold { get; set; }
}
