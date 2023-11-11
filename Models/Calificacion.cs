using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KIM_Style.Models
{
    public class Calificacion
    {

        [Key]
        public int id_calificacion { get; set; }

        [ForeignKey("Producto"), Column(Order = 0)]
        public int cod_producto { get; set; }
        public virtual Producto Producto { get; set; }

        [ForeignKey("Usuario"), Column(Order = 1)]
        public int ced_usuario  { get; set; }
        public  virtual Usuario Usuario{ get; set; }

        [Column(TypeName = "tinyint")]
        public int calificacion {  get; set; }
    }
}
