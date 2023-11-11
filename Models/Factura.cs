using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KIM_Style.Models
{
    public class Factura
    {
        [Key]
        public int id_factura { get; set; }

        [ForeignKey("Usuario")]
        public int id_usuario { get; set; }
        public virtual Usuario Usuario { get; set; }

        public DateTime fecha { get; set; }

        [Column(TypeName = "decimal(11, 2)")]
        public double valor_total { get; set; }

        [MaxLength(60)]
        public string tipo_transaccion { get; set; }

        public string estado_venta {  get; set; }
    }
}
