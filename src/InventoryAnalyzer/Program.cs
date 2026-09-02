using InventoryAnalyzer.Services;

Console.WriteLine("=== Inventory Analyzer ===");
Console.WriteLine("Loading data...");

var categories = DataLoader.LoadCategories();
var products   = DataLoader.LoadProducts(count: 8_000);

Console.WriteLine($"Loaded {categories.Count} categories and {products.Count} products.");

// --- Step 1: find duplicate SKUs ----------------------------------------
// Warning: DuplicateDetector uses an O(n²) algorithm.
// Profiling this block will show a clear CPU hot path in the nested loops.
Console.WriteLine("\nScanning for duplicate SKUs...");
var detector   = new DuplicateDetector();
var duplicates = detector.FindDuplicateSku(products);
Console.WriteLine($"Found {duplicates.Count} products with duplicate SKUs.");

// --- Step 2: generate the category sales report -------------------------
// This will crash because categories 3 and 5 reference each other,
// causing SalesReportService.CalculateSalesSummary to recurse infinitely.
Console.WriteLine("\nGenerating category sales report...");
var reportService = new SalesReportService(categories);
var report        = reportService.CalculateSalesSummary(categoryId: 1);

Console.WriteLine($"\nReport generated for: {report.CategoryName}");
Console.WriteLine($"Total sales: {report.TotalSales:C}");
