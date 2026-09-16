namespace KurtDhylanMotoShopInventory.Models;

public class InventoryItem
{
    public string Name { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public class ServiceItem
{
    public string Name { get; set; } = "";
    public decimal LaborPrice { get; set; }
}
