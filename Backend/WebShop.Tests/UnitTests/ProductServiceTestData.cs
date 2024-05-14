namespace WebShop.Tests.UnitTests;

public class ProductServiceTestData
{
    private static readonly List<int> productCount = new List<int>() { 0, 1, 2, 5, 10 };
    private static readonly List<int> categoryIds = new List<int> { 1, 2, 3 };
    private static readonly List<int> inputPrices = new List<int> { int.MinValue, -10_000, -1, 0, 100, 499, 500, 501, 1000, 10_000, int.MaxValue };
    public static readonly int[] existingPrices = new int[] { 100, 500, 1000 };
    public static readonly Tuple<int, int>[] existingCategoryAndPrices = categoryIds.SelectMany(id => existingPrices.Select(price => new Tuple<int, int>(id, price))).ToArray();
    public static IEnumerable<object[]> ProductCountTestData => productCount.Select(count => new object[] { count });
    public static IEnumerable<object[]> CategoryIdTestData => categoryIds.Select(id => new object[] { id });
    public static IEnumerable<object[]> MinPriceTestData => inputPrices.Select(price => new object[] { price });
    public static IEnumerable<object[]> MaxPriceTestData => inputPrices.Select(price => new object[] { price });
    public static IEnumerable<object[]> PriceTestData => inputPrices.SelectMany(p1 => inputPrices.Select(p2 => new object[] { p1, p2 } ));
    public static IEnumerable<object[]> MixedTestData => categoryIds.SelectMany(id => inputPrices.SelectMany(p1 => inputPrices.Select(p2 => new object[] { id, p1, p2 } )));
};

