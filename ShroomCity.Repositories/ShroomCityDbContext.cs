using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Entities;

namespace ShroomCity.Repositories
{
    public class ShroomCityDbContext : DbContext
    {
        public ShroomCityDbContext(DbContextOptions<ShroomCityDbContext> options) : base(options) {}
        public DbSet<Models.Entities.Attribute> Attributes { get; set; }
        public DbSet<AttributeType> AttributeTypes { get; set; }
        public DbSet<JwtToken> JwtTokens { get; set; }
        public DbSet<Mushroom> Mushrooms { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
    }
}