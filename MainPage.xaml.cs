using System.Collections.ObjectModel;
using KurtDhylanMotoShopInventory.Models;
using KurtDhylanMotoShopInventory.Services;

namespace KurtDhylanMotoShopInventory;

public partial class MainPage : ContentPage
{
    readonly ObservableCollection<InventoryItem> products = new();
    readonly ObservableCollection<ServiceItem> services = new();

    public MainPage()
    {
        InitializeComponent();
        ProductsView.ItemsSource = products;
        ServicesView.ItemsSource = services;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    async Task LoadAsync()
    {
        products.Clear();
        services.Clear();

        foreach (var item in await InventoryStore.LoadProductsAsync())
            products.Add(item);

        foreach (var item in await InventoryStore.LoadServicesAsync())
            services.Add(item);

        UpdateStats();
        FilterProducts();
        FilterServices();
    }

    void UpdateStats()
    {
        ProductCount.Text = products.Count.ToString();
        ServiceCount.Text = services.Count.ToString();
        LowStockCount.Text = products.Count(x => x.Stock <= 5).ToString();
    }

    void FilterProducts()
    {
        var q = ProductSearch.Text?.Trim() ?? "";

        ProductsView.ItemsSource = string.IsNullOrWhiteSpace(q)
            ? products
            : products.Where(x =>
                $"{x.Name} {x.Brand} {x.Model}"
                .Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    void FilterServices()
    {
        var q = ServiceSearch.Text?.Trim() ?? "";

        ServicesView.ItemsSource = string.IsNullOrWhiteSpace(q)
            ? services
            : services.Where(x =>
                x.Name.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    async void AddProduct_Clicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("Add Product", "Product name:");
        if (string.IsNullOrWhiteSpace(name)) return;

        var brand = await DisplayPromptAsync("Add Product", "Brand:") ?? "";
        var model = await DisplayPromptAsync("Add Product", "Model:") ?? "";

        var priceText = await DisplayPromptAsync("Add Product", "Price:");
        decimal.TryParse(priceText, out var price);

        var stockText = await DisplayPromptAsync("Add Product", "Stocks:");
        int.TryParse(stockText, out var stock);

        products.Add(new InventoryItem
        {
            Name = name,
            Brand = brand,
            Model = model,
            Price = price,
            Stock = stock
        });

        await InventoryStore.SaveProductsAsync(products.ToList());
        UpdateStats();
        FilterProducts();
    }

    async void AddService_Clicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("Add Service", "Service / labor name:");
        if (string.IsNullOrWhiteSpace(name)) return;

        var priceText = await DisplayPromptAsync("Add Service", "Labor price:");
        decimal.TryParse(priceText, out var price);

        services.Add(new ServiceItem
        {
            Name = name,
            LaborPrice = price
        });

        await InventoryStore.SaveServicesAsync(services.ToList());
        UpdateStats();
        FilterServices();
    }

    void ProductSearch_TextChanged(object sender, TextChangedEventArgs e) => FilterProducts();

    void ServiceSearch_TextChanged(object sender, TextChangedEventArgs e) => FilterServices();

    async void Refresh_Clicked(object sender, EventArgs e) => await LoadAsync();
}