using Microsoft.EntityFrameworkCore;
using ticktrax_backend.Models;

namespace ticktrax_backend.DataAnnotations
{
    public class TickTraxContext : DbContext
    {
        private readonly IConfiguration _configuration;


        public TickTraxContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Microsoft.EntityFrameworkCore.DbSet<Submission> Submissions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}