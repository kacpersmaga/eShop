using eShop.Models.Domain;
using eShop.Services.Interfaces;

namespace IntegrationTests.Fakes;

public class FaultyItemService : IItemService
{
    public Task<IEnumerable<ShopItem>> GetAllItems()
    {
        throw new Exception("Simulated exception for testing Internal Server Error");
    }
    
    public Task AddItem(ShopItem item)
    {
        throw new Exception("Simulated exception for testing Internal Server Error in AddItem");
    }
}