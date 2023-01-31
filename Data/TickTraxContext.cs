using Microsoft.EntityFrameworkCore;
using ticktrax_backend.Models;

namespace ticktrax_backend.Data
{
    public class TickTraxContext : DbContext
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}