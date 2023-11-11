using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KIM_Style.Models
{
    public class Rol
    {
        [Key]
        [Column(TypeName = "tinyint")]
        public int id_rol { get; set; }
        public string nombre_rol { get; set; }
    }
}
