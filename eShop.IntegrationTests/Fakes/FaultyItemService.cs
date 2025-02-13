using eShop.Models.Domain;
using eShop.Services.Interfaces;

namespace IntegrationTests.Fakes;

public class FaultyItemService : IItemService
{
    public Task<IEnumerable<ShopItem>> GetAllItems()
    {
        // Simulate an error during retrieval.
        throw new Exception("Simulated exception for testing Internal Server Error");
    }
    
    public Task AddItem(ShopItem item)
    {
        // Optionally, throw an exception if you want to simulate errors during addition.
        throw new Exception("Simulated exception for testing Internal Server Error in AddItem");
    }
}