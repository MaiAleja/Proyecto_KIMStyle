using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KIM_Style.Models
{
    public class Tipo_Talla
    {
        [Key]
        [Column(TypeName = "tinyint")]
        public int id_talla { get; set; }
        [MaxLength(100)]
        public string talla { get; set; }
    }
}
