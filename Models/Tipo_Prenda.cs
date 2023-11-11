using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KIM_Style.Models
{
    public class Tipo_Prenda
    {
        [Key]
        [Column(TypeName = "tinyint")]
        public int id_prenda { get; set; }
        [MaxLength(100)]
        public string descripcion { get; set; }
    }
}
