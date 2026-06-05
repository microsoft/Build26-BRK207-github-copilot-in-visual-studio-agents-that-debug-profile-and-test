namespace InventoryAnalyzer.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Explicit list of subcategory IDs as stored in the data source.
    // A circular entry here (A lists B, B lists A) will cause a stack overflow
    // in any recursive traversal that has no cycle detection.
    public List<int> SubcategoryIds { get; set; } = [];
    public List<Product> Products { get; set; } = [];
}
