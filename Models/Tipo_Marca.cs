using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KIM_Style.Models
{
    public class Tipo_Marca
    {
        [Key]
        [Column(TypeName = "tinyint")]
        public int id_marca { get; set; }
        [MaxLength(100)]
        public string descripcion { get; set; }
    }
}
