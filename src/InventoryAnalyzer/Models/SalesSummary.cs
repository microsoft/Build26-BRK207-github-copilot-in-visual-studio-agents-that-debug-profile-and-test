namespace InventoryAnalyzer.Models;

public class SalesSummary
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal DirectSales { get; set; }
    public decimal SubcategorySales { get; set; }
    public decimal TotalSales => DirectSales + SubcategorySales;
    public List<SalesSummary> Subcategories { get; set; } = [];
}
