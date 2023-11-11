using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KIM_Style.Models
{
    public class Usuario
    {
        [Key]
        public int cedula { get; set; }

        [MaxLength(100)]
        public string nombres { get; set; }

        [MaxLength(100)]
        public string apellidos { get; set; }

        public string genero { get; set; }

        [MaxLength(12)]
        public string telefono { get; set;}
        [MaxLength(100)]
        public string correo { get; set;}
        [MaxLength(200)]
        public string? direccion { get; set; }
        [MaxLength(100)]
        public string constrasena { get; set; }

        [ForeignKey("Rol")]
        public int rol_usuario { get; set; }
        public virtual Rol Rol { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[]? FotoPerfil{ get; set; }

        [NotMapped] // Para evitar que se incluya en las migraciones
        [Display(Name = "Foto de Perfil")]
        [DataType(DataType.Upload)]
        public IFormFile? FotoPerfilArchivo { get; set; }
    }
}
