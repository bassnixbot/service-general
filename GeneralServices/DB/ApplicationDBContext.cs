using Microsoft.EntityFrameworkCore;

namespace GeneralServices.DB;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options) { }

    public DbSet<Link> link { get; set; }
}
