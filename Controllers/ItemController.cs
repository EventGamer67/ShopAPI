using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShopAPI.Models;
using ShopAPI.Tools;

namespace ShopAPI.Controllers
{
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;

        public ItemController(ILogger<ItemController> logger)
        {
            _logger = logger;
        }

        [HttpGet("getitems", Name = "getitems")]
        public string GetItems()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var items = db.items.ToList();

                List<object> customItems = new List<object>();

                foreach (var item in items)
                {
                    var itemServices = db.servicesinfo.Where(si => si.itemID == item.itemID).ToList();
                    var newItem = new
                    {
                        itemID = item.itemID,
                        item_categoryID = item.item_categoryID,
                        item_name = item.item_name,
                        item_price = item.item_price,
                        item_image = itemServices.Select(si => si.item_image)
                    };
                    customItems.Add(newItem);
                }
                return JsonConvert.SerializeObject(customItems);
            }
        }

        [HttpPut("updateitem", Name = "updateitem"), Authorize]
        public string UpdateItem([FromBody] string item)
        {
            _logger.Log(LogLevel.Information, "aaa");
            _logger.LogInformation("cathed");
            Console.WriteLine(item);
            try
            {
                _logger.LogInformation("cathed");
                WPFItem WPFItem;
                try
                {
                    WPFItem = JsonConvert.DeserializeObject<WPFItem>(item);
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        _logger.LogInformation(item);

                        Item updateItem = db.items.Where(si => si.itemID == WPFItem.itemID).ToList()[0];
                        _logger.LogInformation(updateItem.item_name);
                        updateItem.item_name = WPFItem.item_name;
                        _logger.LogInformation(updateItem.item_name);
                        updateItem.item_categoryID = WPFItem.item_categoryID;
                        updateItem.item_price = WPFItem.item_price;
                        db.items.Update(updateItem);

                        ItemServiceInfo itemServiceInfo = db.servicesinfo.Where(si => si.itemID == WPFItem.itemID).ToList()[0];
                        itemServiceInfo.item_image = WPFItem.item_image[0];
                        db.servicesinfo.Update(itemServiceInfo);
                        db.SaveChanges();
                    }
                    return "Ok";

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _logger.LogInformation(ex.Message);
                    return ex.Message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.Message;
            }
        }

        [HttpPost("postitem",Name = "postitem"), Authorize]
        public IActionResult PostItem([FromBody] string item)
        {
            if(item is not null)
            {
                try
                {
                    WPFItem newItem = JsonConvert.DeserializeObject<WPFItem>(item);
                    ItemServiceInfo newItemServiceInfo = new ItemServiceInfo(
                        itemServiceID: 0,
                        itemID: newItem.itemID,
                        item_image: newItem.item_image[0]
                    );
                    Item item1 = new Item(
                        itemID: newItem.itemID,
                        item_categoryID: newItem.item_categoryID,
                        item_name: newItem.item_name,
                        item_price: Convert.ToDouble(newItem.item_price)
                    );

                    using (ApplicationContext db = new ApplicationContext())
                    {
                        db.items.Add(item1);
                        db.Add(newItemServiceInfo);
                        db.SaveChanges();
                    }
                    _logger.LogInformation("oK");
                    return new OkResult();
                }
                catch(Exception e)
                {
                    _logger.LogError(e.ToString());
                    return new BadRequestObjectResult(e.Message);
                }
            }
            else
            {
                _logger.LogError("Null");
                return new BadRequestObjectResult("Null Item");
            }
        }

        [HttpDelete("deleteitem/{id}", Name = "deleteitem"), Authorize]
        public string DeleteItem(string id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.servicesinfo.RemoveRange(db.servicesinfo.Where(item => item.itemID == id).ToList());
                    db.SaveChanges();
                    db.items.Remove(db.items.Where(item => item.itemID == id).First());
                    db.SaveChanges();
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _logger.LogInformation(ex.Message);
                _logger.LogInformation(ex.InnerException.Message.ToString());
                return ex.Message;
            }
        }
    }
}
