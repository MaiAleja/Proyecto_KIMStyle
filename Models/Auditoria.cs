using System.ComponentModel.DataAnnotations;

namespace KIM_Style.Models
{
    public class Auditoria
    {
        [Key]
        public int id { get; set; }
        public string usuario { get; set; }
        public string accion {  get; set; }
        public string tabla { get; set; }
        public string valor_anterior { get; set; }
        public string valor_nuevo { get; set; }
        public string sql { get; set; }
        public DateTime fecha { get; set; }
    }
}
