using eShop.Shared.Abstractions;

namespace eShop.Modules.Catalog.Domain.Entities;

public class ShopItem : Entity
{
    private string _name = string.Empty;
    private string? _description;
    private decimal _price;
    private bool _isAvailable = true;
    private string? _imagePath;
    private string _category = "Uncategorized";
    
    private ShopItem() { }
    
    public ShopItem(
        string name, 
        decimal price, 
        string category, 
        string? description = null, 
        string? imagePath = null)
    {
        SetName(name);
        SetPrice(price);
        SetCategory(category);
        _description = description;
        _imagePath = imagePath;
        CreatedAt = DateTime.UtcNow;
    }
    
    public override int Id { get; protected set; }
    public string Name => _name;
    public string? Description => _description;
    public decimal Price => _price;
    public bool IsAvailable => _isAvailable;
    public string? ImagePath => _imagePath;
    public string Category => _category;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters", nameof(name));
        
        _name = name;
        UpdateModifiedDate();
    }

    public void SetDescription(string? description)
    {
        if (description?.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters", nameof(description));
        
        _description = description;
        UpdateModifiedDate();
    }

    public void SetPrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));
        
        _price = price;
        UpdateModifiedDate();
    }

    public void SetCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty", nameof(category));
        
        _category = category;
        UpdateModifiedDate();
    }

    public void SetImagePath(string? imagePath)
    {
        _imagePath = imagePath;
        UpdateModifiedDate();
    }

    public void Enable() 
    {
        _isAvailable = true;
        UpdateModifiedDate();
    }

    public void Disable() 
    {
        _isAvailable = false;
        UpdateModifiedDate();
    }

    private void UpdateModifiedDate()
    {
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(_name) && 
               _price >= 0 && 
               !string.IsNullOrEmpty(_category);
    }
}