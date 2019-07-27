using Microsoft.EntityFrameworkCore;
namespace Electro.Models
{
    public class ElectroContext : DbContext
    {
        public ElectroContext(DbContextOptions<ElectroContext> options) : base(options){

        }
        public DbSet<Resistencia> Resistencias {get;set;}
        public DbSet<Paralelo> Paralelos {get;set;}        
    }
}