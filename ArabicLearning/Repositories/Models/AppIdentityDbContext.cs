using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ArabicLearning.Repositories.Models
{
    public class AppIdentityDbContext: IdentityDbContext<AppIdentityUser, AppIdentityRole, string>
    {
        private readonly DbContextOptions _options;
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options):base(options)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
        }
    }
}
