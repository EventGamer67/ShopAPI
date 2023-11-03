using Microsoft.EntityFrameworkCore;
using ShopAPI.Models;

namespace ShopAPI.Tools
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Category> categories { get; set; }
        public DbSet<Item> items { get; set; }
        public DbSet<ItemServiceInfo> servicesinfo { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<Permission> permissions { get; set; }
        public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string con = "server=localhost;user=root;password=danil672004;database=restourant;port=3306;SslMode=none;Allow User Variables =true";
            optionsBuilder.UseMySql(con, ServerVersion.AutoDetect(con));
        }
    }
}