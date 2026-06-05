namespace InventoryAnalyzer.Services;

using InventoryAnalyzer.Models;

public class SalesReportService
{
    private readonly Dictionary<int, Category> _categories;

    public SalesReportService(List<Category> categories)
    {
        _categories = categories.ToDictionary(c => c.Id);
    }

    // Recursively aggregates sales figures across the category hierarchy.
    //
    // Bug: no cycle detection.
    // If any category's SubcategoryIds form a cycle (e.g. 3 -> 5 -> 3),
    // this method recurses infinitely and terminates with a StackOverflowException.
    public SalesSummary CalculateSalesSummary(int categoryId)
    {
        var category = _categories[categoryId];

        var summary = new SalesSummary
        {
            CategoryName = category.Name,
            DirectSales = category.Products.Sum(p => p.UnitPrice * p.UnitsSold),
        };

        foreach (var subcategoryId in category.SubcategoryIds)
        {
            var subcategorySummary = CalculateSalesSummary(subcategoryId);
            summary.SubcategorySales += subcategorySummary.TotalSales;
            summary.Subcategories.Add(subcategorySummary);
        }

        return summary;
    }
}
