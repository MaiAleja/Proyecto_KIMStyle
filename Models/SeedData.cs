using KIM_Style.Controllers;
using KIM_Style.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace KIM_Style.Models
{
        public class SeedData
        {
            public static void Initialize(IServiceProvider serviceProvider)
            {
                using (var context = new KIM_StyleContext(
                    serviceProvider.GetRequiredService<DbContextOptions<KIM_StyleContext>>()))
                {

                    if (!context.Rol.Any())
                    {
                        context.Rol.AddRange(
                            new Rol
                            {
                                nombre_rol = "Admin"
                            },
                        new Rol
                        {
                            nombre_rol = "Empleado"
                        },
                        new Rol
                        {
                            nombre_rol = "Cliente"
                        }
                            );
                        context.SaveChanges();
                    }
                    if (!context.Usuario.Any())
                    {
                    VerificacionesServicio _verServicio = new VerificacionesServicio(context);
                    context.Usuario.Add(
                        new Usuario
                        {
                            cedula = 1,
                            nombres = "Admin",
                            apellidos = "Admin",
                            constrasena = _verServicio.EncriptarContrasena("123456"),
                            correo = "Admin@ejemplo.com",
                            genero = "-",
                            rol_usuario = 1,
                            telefono = "3213213213"
                        }
                        );
                    context.SaveChanges();
                    }
                }
            }
        }
}
