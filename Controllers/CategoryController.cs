using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopAPI.Models;
using ShopAPI.Tools;

namespace ShopAPI.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ILogger<CategoryController> logger)
        {
            _logger = logger;
        }

        [HttpPost("addcategory", Name = "addcategory"), Authorize]
        public ActionResult PostCategory([FromBody] string categoryJson)
        {
            Category category;
            try
            {
                category = JsonConvert.DeserializeObject<Category>(categoryJson);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new BadRequestObjectResult(e.Message);
            }
            if (category is not null)
            {
                try
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        db.categories.Add(category);
                        db.SaveChanges();
                    }
                    _logger.LogInformation("oK");
                    return new OkResult();
                }
                catch (Exception e)
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

        [HttpDelete("deletecategory/{id}", Name = "deletecategory"), Authorize]
        public string DeleteCategory(string id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.categories.Remove(db.categories.Where(category => category.categoryID == id).ToList()[0]);
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

        [HttpGet("getcategories", Name = "getcategories")]
        public string GetCategories()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var categories = db.categories.ToList();
                return JsonConvert.SerializeObject(categories);
            }
        }
    }
}
