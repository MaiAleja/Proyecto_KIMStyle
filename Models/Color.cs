using System.ComponentModel.DataAnnotations;

namespace KIM_Style.Models
{
    public class Color
    {
        [Key]
        public int id_color { get; set; }
        public string color_nombre { get; set; }
    }
}
