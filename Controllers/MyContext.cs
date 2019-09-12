using Microsoft.EntityFrameworkCore;
 
namespace WeddingPlanner.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> users {get;set;} // Notice "<User>" reffers to the Model (i.e. Models/User.cs) but "users" reffers to the table name
        public DbSet<Wedding> weddings {get;set;}
        public DbSet<Association> associations {get;set;}
    }
}
