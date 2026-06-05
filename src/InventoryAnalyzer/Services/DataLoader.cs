namespace InventoryAnalyzer.Services;

using InventoryAnalyzer.Models;

public static class DataLoader
{
    public static List<Category> LoadCategories()
    {
        // Simulates loading category hierarchy from an external data source.
        //
        // Bug: Category 3 (Laptops) lists Category 5 (Gaming Laptops) as a subcategory,
        // and Category 5 lists Category 3 as a subcategory.
        // This creates a cycle: 3 -> 5 -> 3 -> 5 -> ...
        // Any recursive traversal without cycle detection will overflow the call stack.
        return
        [
            new() { Id = 1, Name = "Electronics",     SubcategoryIds = [2, 6, 7] },
            new() { Id = 2, Name = "Computers",       SubcategoryIds = [3, 4] },
            new() { Id = 3, Name = "Laptops",         SubcategoryIds = [5] },        // Bug: creates cycle 3 -> 5 -> 3
            new() { Id = 4, Name = "Desktops",        SubcategoryIds = [] },
            new() { Id = 5, Name = "Gaming Laptops",  SubcategoryIds = [3] },        // Bug: creates cycle 5 -> 3 -> 5
            new() { Id = 6, Name = "Phones",          SubcategoryIds = [] },
            new() { Id = 7, Name = "Accessories",     SubcategoryIds = [] },
        ];
    }

    public static List<Product> LoadProducts(int count = 5000)
    {
        var random = new Random(42);
        var skuPool = Enumerable.Range(1, count / 4)
            .Select(i => $"SKU-{i:D5}")
            .ToArray();

        return Enumerable.Range(1, count)
            .Select(i => new Product
            {
                Id = i,
                Sku = skuPool[random.Next(skuPool.Length)],
                Name = $"Product {i}",
                UnitPrice = Math.Round((decimal)(random.NextDouble() * 999 + 1), 2),
                CategoryId = random.Next(1, 8),
                UnitsSold = random.Next(0, 200),
            })
            .ToList();
    }
}
