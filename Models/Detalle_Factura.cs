using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KIM_Style.Models
{
    public class Detalle_Factura
    {
        [Key]
        public int id_detalle { get; set; }

        public int cantidad { get; set; }

        [Column(TypeName = "decimal(11, 2)")]
        public double total_producto { get; set; }

        [ForeignKey("Factura")]
        public int id_factura { get; set; }
        public virtual Factura Factura { get; set; }

        [ForeignKey("Producto")]
        public int cod_producto { get; set; }
        public virtual Producto Producto { get; set; }
    }
}
