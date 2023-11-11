using KIM_Style.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace KIM_Style.Data
{
    public class KIM_StyleContext:DbContext
    {
        public KIM_StyleContext(DbContextOptions<KIM_StyleContext> options):base(options) { }
        public DbSet<Auditoria> Auditoria { get; set; }
        public DbSet<Calificacion> Calificacion { get; set; }
        public DbSet<Color> Color { get; set; }
        public DbSet<Detalle_Factura> Detalle_Factura {  get; set; }
        public DbSet <Factura> Factura { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Tipo_Marca> Tipo_Marca { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Tipo_Prenda> Tipo_Prenda { get; set; }
        public DbSet<Tipo_Talla> Tipo_Talla { get; set; }
        public DbSet<Usuario> Usuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .Property(u => u.cedula)
                .ValueGeneratedNever();
        }

    }
}
