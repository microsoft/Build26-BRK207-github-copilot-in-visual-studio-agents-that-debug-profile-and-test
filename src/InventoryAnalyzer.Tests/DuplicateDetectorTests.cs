namespace InventoryAnalyzer.Tests;

using InventoryAnalyzer.Models;
using InventoryAnalyzer.Services;

[TestClass]
public class DuplicateDetectorTests
{
    private readonly DuplicateDetector _detector = new();

    [TestMethod]
    public void FindDuplicateSku_WithAllUniqueProducts_ReturnsEmpty()
    {
        var products = Enumerable.Range(1, 20)
            .Select(i => new Product { Id = i, Sku = $"SKU-{i:D5}" })
            .ToList();

        var result = _detector.FindDuplicateSku(products);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void FindDuplicateSku_WithKnownDuplicates_ReturnsCorrectProducts()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Sku = "SKU-001", Name = "Widget A" },
            new() { Id = 2, Sku = "SKU-002", Name = "Widget B" },
            new() { Id = 3, Sku = "SKU-001", Name = "Widget A (copy)" },   // duplicate
            new() { Id = 4, Sku = "SKU-003", Name = "Widget C" },
            new() { Id = 5, Sku = "SKU-002", Name = "Widget B (copy)" },   // duplicate
            new() { Id = 6, Sku = "SKU-004", Name = "Widget D" },
        };

        var result = _detector.FindDuplicateSku(products);

        Assert.AreEqual(2, result.Count);
        CollectionAssert.Contains(result.Select(p => p.Sku).ToList(), "SKU-001");
        CollectionAssert.Contains(result.Select(p => p.Sku).ToList(), "SKU-002");
    }

    // This test exposes the O(n²) performance issue.
    // Run it under the Visual Studio CPU Profiler to see the hot path inside the
    // nested loop. The Copilot profiler agent can explain why it is slow and suggest
    // a HashSet<string>-based O(n) replacement.
    [TestMethod]
    [Timeout(60_000)]
    public void FindDuplicateSku_WithLargeCatalog_CompletesAndReturnsDuplicates()
    {
        var products = DataLoader.LoadProducts(count: 10_000);

        var result = _detector.FindDuplicateSku(products);

        Assert.IsTrue(result.Count > 0, "Expected duplicate SKUs in the generated catalog.");
    }
}
