using Microsoft.EntityFrameworkCore;
using ticktrax_backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ticktrax_backend.Data
{
    public class TickTraxContext : IdentityDbContext<User>
    {
        private readonly IConfiguration _configuration;

        public TickTraxContext(IConfiguration configuration, DbContextOptions<TickTraxContext> options) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var connectionString = _configuration.GetConnectionString("tickTraxDbContext");

            optionsBuilder.EnableDetailedErrors();
            //uses options builder and mysql with connectionstring

        }

        public Microsoft.EntityFrameworkCore.DbSet<Submission> Submissions { get; set; }

        public Microsoft.EntityFrameworkCore.DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Submission>()
                .HasOne(sub => sub.Owner)
                .WithMany(user => user.UserSubmissions)
                .HasForeignKey(sub => sub.OwnerId);
        }
    }
}