using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Construction;
using Microsoft.Extensions.Logging;
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
        public ActionResult PostCategory([FromBody] Category category)
        {
            if (category is not null)
            {
                try
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        db.categories.Add(category);
                        db.SaveChanges();
                    }
                    return new OkResult();
                }
                catch (Exception e)
                {
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

        [HttpPut("updatecategory", Name = "updatecategory"), Authorize]
        public ActionResult putCategory([FromBody] Category category)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                List<Category> cat = db.categories.Where(cate => cate.categoryID == category.categoryID).ToList();
                if(cat != null && cat.Count > 0)
                {
                    cat[0].category_name = category.category_name;
                    db.SaveChanges();
                    return Ok();
                }
                _logger.LogCritical("not found");
                return BadRequest("Category doesnt exist");
            }
        }
    }
}
