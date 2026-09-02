# InventoryAnalyzer — Demo for BRK207

A minimal .NET 10 console application that intentionally contains two bugs,
designed to showcase the GitHub Copilot **debug agent** and **CPU profiler** in Visual Studio.

## Solution structure

```
src/
  InventoryAnalyzer/          ← console app (crashes on startup)
  InventoryAnalyzer.Tests/    ← MSTest project (reproduces both issues)
  InventoryAnalyzer.slnx
```

---

## Bug 1 — StackOverflowException (debug agent demo)

**Where:** `Services/SalesReportService.cs` → `CalculateSalesSummary`

**What happens:** `DataLoader.LoadCategories` returns a category list where
Category 3 (Laptops) and Category 5 (Gaming Laptops) list each other as
subcategories, forming a cycle `3 → 5 → 3 → 5 → …`.  
`CalculateSalesSummary` recurses through `SubcategoryIds` with no cycle
detection, so the call stack grows until the process crashes.

**How to reproduce:**
1. Run the console app (`F5` or `dotnet run`) — it crashes after printing the
   duplicate-scan results.
2. Or run `SalesReportServiceTests.CalculateSalesSummary_WithCircularSubcategoryReference_CausesStackOverflow`
   in the test runner — the test process aborts.

**Copilot demo steps:**
- Open the crashed process / failed test in Visual Studio.
- Ask the Copilot debug agent: *"Why did this crash? What is the root cause?"*
- Copilot inspects the call stack and identifies the circular `SubcategoryIds`
  reference in the loaded data.
- Ask: *"How should I fix this?"*  
  Expected fix: add a `HashSet<int> visited` parameter to detect cycles.

---

## Bug 2 — O(n²) duplicate detection (profiler demo)

**Where:** `Services/DuplicateDetector.cs` → `FindDuplicateSku`

**What happens:** Instead of using a `HashSet<string>`, the method maintains a
`List<string>` of seen SKUs and does a linear scan for each product.  
On a 10,000-product catalog this runs ~50 million string comparisons.

**How to reproduce:**
1. Run the app — it pauses noticeably on the *"Scanning for duplicate SKUs…"* step.
2. Or profile `DuplicateDetectorTests.FindDuplicateSku_WithLargeCatalog_CompletesAndReturnsDuplicates`.

**Copilot demo steps:**
- Run the app or the test under the Visual Studio CPU profiler.
- The flame graph shows the hot path in `DuplicateDetector.FindDuplicateSku`.
- Ask the Copilot profiler agent: *"Why is this method slow? How do I fix it?"*
- Expected fix: replace the inner `List<string>` with `HashSet<string>`.

---

## Running the solution

```bash
# from the src/ directory
dotnet run --project InventoryAnalyzer
dotnet test InventoryAnalyzer.Tests
```
