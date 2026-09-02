namespace InventoryAnalyzer.Services;

using InventoryAnalyzer.Models;

public class DuplicateDetector
{
    // Performance issue: O(n²) — for each product, scans the entire seen-list linearly.
    // On a catalog of 10,000 products this executes ~50 million comparisons.
    // A HashSet<string> lookup would reduce this to O(n) and run orders of magnitude faster.
    public List<Product> FindDuplicateSku(List<Product> products)
    {
        var seen = new List<string>();
        var duplicates = new List<Product>();

        for (int i = 0; i < products.Count; i++)
        {
            bool alreadySeen = false;

            for (int j = 0; j < seen.Count; j++)
            {
                if (seen[j] == products[i].Sku)
                {
                    alreadySeen = true;
                    break;
                }
            }

            if (alreadySeen)
            {
                duplicates.Add(products[i]);
            }
            else
            {
                seen.Add(products[i].Sku);
            }
        }

        return duplicates;
    }
}
