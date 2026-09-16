using System.Text.Json;
using KurtDhylanMotoShopInventory.Models;

namespace KurtDhylanMotoShopInventory.Services;

public static class InventoryStore
{
    static string ProductsFile => Path.Combine(FileSystem.AppDataDirectory, "products.json");
    static string ServicesFile => Path.Combine(FileSystem.AppDataDirectory, "services.json");

    public static async Task<List<InventoryItem>> LoadProductsAsync() =>
        await LoadAsync<InventoryItem>(ProductsFile);

    public static async Task<List<ServiceItem>> LoadServicesAsync() =>
        await LoadAsync<ServiceItem>(ServicesFile);

    public static Task SaveProductsAsync(List<InventoryItem> items) =>
        SaveAsync(ProductsFile, items);

    public static Task SaveServicesAsync(List<ServiceItem> items) =>
        SaveAsync(ServicesFile, items);

    static async Task<List<T>> LoadAsync<T>(string file)
    {
        if (!File.Exists(file)) return new();

        try
        {
            var json = await File.ReadAllTextAsync(file);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }
        catch
        {
            return new();
        }
    }

    static Task SaveAsync<T>(string file, T data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        return File.WriteAllTextAsync(file, json);
    }
}
