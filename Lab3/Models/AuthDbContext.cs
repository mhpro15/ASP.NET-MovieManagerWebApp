
using Microsoft.EntityFrameworkCore;

namespace Lab3.Model
{
    public class AuthDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }
    }
}
