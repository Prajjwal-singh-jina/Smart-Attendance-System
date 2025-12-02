using bolonotoproxy.Models;
using Microsoft.EntityFrameworkCore;
namespace bolonotoproxy.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) :base(option)   
        {
            
        }
        public DbSet<Student> Students { get; set; }  
        public DbSet<Sign_up> register { get; set; }
        public DbSet<UserToken> UserTokens {  get; set; }
        public DbSet<Attendence> Mark_Attendence {  get; set; }  
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // This tells EF Core: "Do not try to make a SQL column for 'Image'"
            modelBuilder.Entity<Student>().Ignore(t => t.ImageFile);
        }
    }
}
