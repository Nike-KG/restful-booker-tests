using System.Text.Json;

namespace RestfulBooker.Tests.Utils;

public static class TestDataLoader<T>
{
    public static IEnumerable<object[]> Load(string relativePath)
    {
        var json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "TestData", relativePath));
        var items = JsonSerializer.Deserialize<List<T>>(json);
        foreach (var item in items!)
        {
            yield return new object[] { item! };
        }
    }
}
