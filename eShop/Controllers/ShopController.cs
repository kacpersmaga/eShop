using eShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers;

public class ShopController : Controller
{
    private readonly IItemService _itemService;
    private readonly IBlobStorageService _blobStorageService;

    public ShopController(IItemService itemService, IBlobStorageService blobStorageService)
    {
        _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
        _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
    }

    public async Task<IActionResult> Index()
    {
        var items = await _itemService.GetAllItems();
        
        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                item.ImagePath = _blobStorageService.GetBlobSasUri(item.ImagePath);
            }
        }

        return View(items);
    }
}
