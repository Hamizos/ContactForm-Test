using Contact.Models;
using Microsoft.EntityFrameworkCore;

namespace Contact.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ContactModel> Contacts { get; set; }
    }
}
