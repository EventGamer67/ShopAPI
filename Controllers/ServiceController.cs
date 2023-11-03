using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopAPI.Tools;

namespace ShopAPI.Controllers
{
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(ILogger<ServiceController> logger)
        {
            this._logger = logger;
        }
            
        [HttpGet("Ping")]
        public string Ping()
        {
            //using (ApplicationContext db = new ApplicationContext())
            //{
            //    db.Database.CanConnect();
            //}
            return "Pong";
        }
    }
}
