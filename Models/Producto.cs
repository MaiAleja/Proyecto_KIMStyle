using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KIM_Style.Models
{
    public class Producto
    {
        [Key]
        public int cod_producto { get; set; }
        public string temporada { get; set; }
        public string material { get; set; }
        public string genero { get; set; }
        public int cantidad { get; set; }

        public int calificacion_promedio { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[]? Imagen { get; set; }

        [Column(TypeName = "decimal(11, 2)")]
        public double valor {  get; set; }

        [ForeignKey("Color")]
        public int id_color { get; set; }
        public virtual Color Color { get; set; }

        [ForeignKey("Tipo_Prenda")]
        public int id_tipo_prenda { get; set; }
        public virtual Tipo_Prenda Tipo_Prenda { get; set; }

        [ForeignKey("Tipo_Marca")]
        public int marca { get; set; }
        public virtual Tipo_Marca Tipo_Marca { get; set; }

        [ForeignKey("Tipo_Talla")]
        public int talla { get; set; }
        public virtual Tipo_Talla Tipo_Talla { get; set; }

    }
}
