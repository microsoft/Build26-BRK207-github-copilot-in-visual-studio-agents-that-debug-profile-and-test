namespace InventoryAnalyzer.Tests;

using InventoryAnalyzer.Models;
using InventoryAnalyzer.Services;

[TestClass]
public class SalesReportServiceTests
{
    [TestMethod]
    public void CalculateSalesSummary_WithValidHierarchy_AggregatesSalesCorrectly()
    {
        var categories = new List<Category>
        {
            new()
            {
                Id = 1, Name = "Root", SubcategoryIds = [2, 3],
                Products = [new Product { Id = 1, Sku = "R-001", UnitPrice = 10m, UnitsSold = 5 }],
            },
            new()
            {
                Id = 2, Name = "Child A", SubcategoryIds = [],
                Products = [new Product { Id = 2, Sku = "A-001", UnitPrice = 20m, UnitsSold = 3 }],
            },
            new()
            {
                Id = 3, Name = "Child B", SubcategoryIds = [],
                Products = [new Product { Id = 3, Sku = "B-001", UnitPrice = 15m, UnitsSold = 4 }],
            },
        };

        var service = new SalesReportService(categories);
        var summary = service.CalculateSalesSummary(categoryId: 1);

        Assert.AreEqual("Root", summary.CategoryName);
        Assert.AreEqual(50m,  summary.DirectSales);        // 10 * 5
        Assert.AreEqual(120m, summary.SubcategorySales);   // (20*3) + (15*4) = 60 + 60
        Assert.AreEqual(170m, summary.TotalSales);
        Assert.AreEqual(2,    summary.Subcategories.Count);
    }

    // Reproduces the production crash.
    //
    // Category 3 (Laptops) and Category 5 (Gaming Laptops) list each other as subcategories.
    // SalesReportService.CalculateSalesSummary has no cycle detection, so it recurses
    // infinitely until a StackOverflowException terminates the process.
    //
    // Use the GitHub Copilot debug agent to inspect the call stack and identify the
    // circular reference as the root cause.
    [TestMethod]
    public void CalculateSalesSummary_WithCircularSubcategoryReference_CausesStackOverflow()
    {
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Electronics",    SubcategoryIds = [2] },
            new() { Id = 2, Name = "Computers",      SubcategoryIds = [3] },
            new() { Id = 3, Name = "Laptops",        SubcategoryIds = [5] },   // Bug: cycle starts here
            new() { Id = 5, Name = "Gaming Laptops", SubcategoryIds = [3] },   // Bug: points back to 3
        };

        var service = new SalesReportService(categories);

        // This call will overflow the stack and crash the test runner process.
        // There is no try/catch here intentionally — the crash IS the scenario to investigate.
        var report = service.CalculateSalesSummary(categoryId: 1);

        Assert.Fail("Expected a StackOverflowException but the call returned unexpectedly.");
    }
}
