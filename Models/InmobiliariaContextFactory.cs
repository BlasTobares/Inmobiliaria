using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Inmobiliaria.Models
{
    public class InmobiliariaContextFactory : IDesignTimeDbContextFactory<InmobiliariaContext>
    {
        public InmobiliariaContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InmobiliariaContext>();
            optionsBuilder.UseSqlite("Data Source=inmobiliaria.db");

            return new InmobiliariaContext(optionsBuilder.Options);
        }
    }
}
