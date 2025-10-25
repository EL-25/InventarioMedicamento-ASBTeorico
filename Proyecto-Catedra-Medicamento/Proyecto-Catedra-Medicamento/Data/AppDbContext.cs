using Microsoft.EntityFrameworkCore;
using Proyecto_Catedra_Medicamento.Models;

namespace Proyecto_Catedra_Medicamento.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Entrada> Entradas { get; set; }
        public DbSet<Salida> Salidas { get; set; }
    }
}
