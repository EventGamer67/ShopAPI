using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using ShopAPI.Models;
using ShopAPI.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShopAPI.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceController> _logger;

        public AuthController(IConfiguration configuration, ILogger<ServiceController> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
        }

        [HttpPost("login")]
        public ActionResult<Tuple<User,string>> Login([FromBody] UserDto request)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                User? user = db.users.FirstOrDefault(user => user.user_name == request.user_name);
                if (user != null)
                {
                    var sha1 = new HMACSHA256(Encoding.UTF8.GetBytes(AuthOptions.KEY));
                    var sha1data = sha1.ComputeHash(Encoding.UTF8.GetBytes(request.password));
                    var hashedPassword = UTF8Encoding.UTF8.GetString(sha1data);
                    if (user.user_passwordHash == hashedPassword)
                    {
                        string token = CreateToken(user);
                        _logger.LogInformation((user, token).ToString());
                        return (user, token).ToTuple();
                    }
                    else
                    {
                        return BadRequest("Wrong password");
                    }
                }
                else
                {
                    return BadRequest("User doesnt exist");
                }
            }
        }

        [HttpPost("register")]
        public ActionResult<Tuple<User, string>> Register([FromBody] UserDto request)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if(db.users.Any(user => user.user_name == request.user_name))
                {
                    return BadRequest("User exist");
                }
            }

            var sha1 = new HMACSHA256(Encoding.UTF8.GetBytes(AuthOptions.KEY));
            var sha1data = sha1.ComputeHash(Encoding.UTF8.GetBytes(request.password));
            var hashedPassword = UTF8Encoding.UTF8.GetString(sha1data);
            _logger.LogInformation(hashedPassword);

            User user = new User(
                userID: 1,
                user_roleID: 1,
                user_name: request.user_name,
                user_passwordHash: hashedPassword
            );

            string token = CreateToken(user);
            
            using (ApplicationContext db = new ApplicationContext())
            {
                Role role = new Role();
                role.role_name = "Guest";
                role.roleID = 1;
                db.roles.Add(role);
                db.SaveChanges();
                db.users.Add(user);
                db.SaveChanges();
            }
            return (user, token).ToTuple();
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, user.user_name) };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
