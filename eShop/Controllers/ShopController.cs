using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers;

public class ShopController : Controller
{
    private readonly ItemService _itemService;
    private readonly BlobStorageService _blobStorageService;

    public ShopController(ItemService itemService, BlobStorageService blobStorageService)
    {
        _itemService = itemService;
        _blobStorageService = blobStorageService;
    }

    public IActionResult Index()
    {
        var items = _itemService.GetAllItems();
        
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