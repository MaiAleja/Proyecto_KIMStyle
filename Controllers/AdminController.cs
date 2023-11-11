using KIM_Style.Data;
using KIM_Style.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KIM_Style.Controllers
{
    public class AdminController : Controller
    {
        private readonly KIM_StyleContext _context;
        private readonly VerificacionesServicio _verServicio;
        public AdminController(KIM_StyleContext context)
        {
            this._context = context;
            this._verServicio = new VerificacionesServicio(context);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index(string error)
        {
            ViewBag.error = error;
            //Pasar lista de Tallas de la BD a la vista
            List<Tipo_Talla> listaTallas = _context.Tipo_Talla.ToList();
            ViewBag.listaTallas = listaTallas;
            //Pasar lista de Marcas de la BD a la vista
            List<Tipo_Marca> listaMarcas = _context.Tipo_Marca.ToList();
            ViewBag.listaMarcas = listaMarcas;
            //Pasar lista de Colores de la BD a la vista
            List<Color> listaColores = _context.Color.ToList();
            ViewBag.listaColores = listaColores;
            //Pasar lista de Marcas de la BD a la vista
            List<Tipo_Prenda> listaTipoPrendas = _context.Tipo_Prenda.ToList();
            ViewBag.listaTipoPrendas = listaTipoPrendas;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Registro_Empleado(string error)
        {
            ViewBag.error = error;
            List<Usuario> listaUsuarios = _context.Usuario.Include(u => u.Rol).ToList();
            ViewBag.listaUsuarios = listaUsuarios;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Agregar_Empleado(string error,string accion = "Agregar",string metodo = "Registrar_empleado")
        {
            ViewBag.metodo = metodo;
            ViewBag.accion = accion;
            ViewBag.error = error;
            Usuario usuario = new Usuario();
            if (TempData["usuario"] != null)
            {
                usuario = JsonConvert.DeserializeObject<Usuario>(TempData["usuario"].ToString());
            }
            return View(usuario);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Registrar_Empleado(Usuario usuario)
        {
            TempData["usuario"] = JsonConvert.SerializeObject(usuario);
            string error = _verServicio.verificacionUsuario(usuario);
            if (string.IsNullOrEmpty(error))
            {
                usuario.rol_usuario = 2;
                TempData["usuario"] = null;
                usuario.constrasena = _verServicio.EncriptarContrasena(usuario.constrasena);
                _context.Usuario.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction("Registro_Empleado", "Admin");
            }
            return RedirectToAction("Agregar_Empleado", "Admin" , new { error = error, });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Modificar_Empleado(int id_usuario,string accion)
        {
            Usuario usuario = _context.Usuario.FirstOrDefault(u => u.cedula == id_usuario);
            if (usuario == null)
            {
                return RedirectToAction("Registro_Empleado", new { error = "Esa Cedula No Está Registrada" });
            }
            if (accion == "Actualizar")
            {
                TempData["cedula"] = usuario.cedula;
                TempData["usuario"] = JsonConvert.SerializeObject(usuario);
                return RedirectToAction("Agregar_Empleado", "Admin", new {accion = "Actualizar", metodo = "Actualizar_Empleado" });
            }
            if (accion == "Eliminar")
            {
                return RedirectToAction("Eliminar_Empleado", "Admin", new { id_usuario = id_usuario });
            }
            return RedirectToAction("Index", new { error = "Accion aparentemente no registrada" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Actualizar_Empleado(Usuario usuario,string conservarContrasena)
        {
            TempData["usuario"] = JsonConvert.SerializeObject(usuario);
            string error = _verServicio.verificacionUsuario(usuario,true,conservarContrasena);
            if (!string.IsNullOrEmpty(error))
            {
                return RedirectToAction("Agregar_Empleado", "Admin", new { error = error, accion = "Actualizar", metodo = "Actualizar_Empleado" });
            }
            Usuario usuarioBD = _context.Usuario.FirstOrDefault(u => u.cedula == int.Parse(TempData["cedula"].ToString()));
            TempData["cedula"] = null;
            if(usuarioBD.cedula != usuario.cedula)
            {
                if(_context.Usuario.FirstOrDefault(u => u.cedula == usuario.cedula) != null)
                {
                    error = "Ya existe un usuario con esa cedula";
                    return RedirectToAction("Agregar_Empleado", "Admin", new { error = error, accion = "Actualizar", metodo = "Actualizar_Empleado" });
                }
                if (conservarContrasena != null)
                {
                    usuario.constrasena = _verServicio.EncriptarContrasena(usuarioBD.constrasena);
                }
                usuario.rol_usuario = usuarioBD.rol_usuario;
                _context.Usuario.Remove(usuarioBD);
                _context.Usuario.Add(usuario);
                _context.SaveChanges();
            }
            else
            {
                usuarioBD.nombres = usuario.nombres;
                usuarioBD.apellidos = usuario.apellidos;
                usuarioBD.correo = usuario.correo;
                if (conservarContrasena == null)
                {
                    usuarioBD.constrasena = usuario.constrasena;
                }
                usuarioBD.telefono = usuario.telefono;
                usuarioBD.direccion = usuario.direccion;
                usuarioBD.genero = usuario.genero;
                _context.SaveChanges();
            }
            TempData["usuario"] = null;
            return RedirectToAction("Registro_Empleado", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Eliminar_Empleado(int id_usuario)
        {
            Usuario usuario = _context.Usuario.FirstOrDefault(u => u.cedula == id_usuario);
            _context.Usuario.Remove(usuario);
            _context.SaveChanges();
            return RedirectToAction("Registro_Empleado", new { error = "Eliminado correctamente"});
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Agregar_Talla(string nombre_talla) {
            if(string.IsNullOrWhiteSpace(nombre_talla)) {
                string error = "Error agregando talla, estaba vacío";
                return RedirectToAction("Index", new { error  = error }) ;
            }

            _context.Tipo_Talla.Add(
                new Tipo_Talla 
                {
                    talla = nombre_talla.Trim()
                }
                );
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Agregar_Color(string nombre_color)
        {
            if (string.IsNullOrWhiteSpace(nombre_color))
            {
                string error = "Error agregando color, estaba vacío";
                return RedirectToAction("Index", new { error = error });
            }

            _context.Color.Add(
                new Color
                {
                    color_nombre = nombre_color.Trim()
                }
                );
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Agregar_Marca(string nombre_marca)
        {
            if (string.IsNullOrWhiteSpace(nombre_marca))
            {
                string error = "Error agregando color, estaba vacío";
                return RedirectToAction("Index", new { error = error });
            }

            _context.Tipo_Marca.Add(
                new Tipo_Marca
                {
                    descripcion = nombre_marca.Trim()
                }
                );
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Agregar_Tipo_Prenda(string nombre_tipo_prenda)
        {
            if (string.IsNullOrWhiteSpace(nombre_tipo_prenda))
            {
                string error = "Error agregando color, estaba vacío";
                return RedirectToAction("Index", new { error = error });
            }

            _context.Tipo_Prenda.Add(
                new Tipo_Prenda
                {
                    descripcion = nombre_tipo_prenda.Trim()
                }
                );
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Eliminar_Talla(int id_talla)
        {
            if (id_talla < 1)
            {
                string error = "Error eliminando talla, No esta dentro del rango";
                return RedirectToAction("Index", new { error = error});
            }
            var talla = _context.Tipo_Talla.FirstOrDefault(t => t.id_talla == id_talla);
            if(talla == null)
            {
                string error = "Error eliminando talla, El id no coincide con ninguna talla";
                return RedirectToAction("Index", new { error = error });
            }
            _context.Tipo_Talla.Remove(talla);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Eliminar_Marca(int id_marca)
        {
            if (id_marca == null || id_marca < 1)
            {
                string error = "Error eliminando la Marca, No esta dentro del rango";
                return RedirectToAction("Index", new { error = error });
            }
            var marca = _context.Tipo_Marca.FirstOrDefault(t => t.id_marca == id_marca);
            if (marca == null)
            {
                string error = "Error eliminando la marca, El id no coincide con ninguna marca";
                return RedirectToAction("Index", new { error = error });
            }
            _context.Tipo_Marca.Remove(marca);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Eliminar_Color(int id_color)
        {
            if (id_color == null || id_color < 1)
            {
                string error = "Error eliminando el color, No esta dentro del rango";
                return RedirectToAction("Index", new { error = error });
            }
            var color = _context.Color.FirstOrDefault(t => t.id_color == id_color);
            if (color == null)
            {
                string error = "Error eliminando el color, El id no coincide con ningun color";
                return RedirectToAction("Index", new { error = error });
            }
            _context.Color.Remove(color);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Eliminar_Tipo_Prenda(int id_tipo_prenda)
        {
            if (id_tipo_prenda == null || id_tipo_prenda < 1)
            {
                string error = "Error eliminando el tipo de prenda, No esta dentro del rango";
                return RedirectToAction("Index", new { error = error });
            }
            var tipo_prenda = _context.Tipo_Prenda.FirstOrDefault(t => t.id_prenda == id_tipo_prenda);
            if (tipo_prenda == null)
            {
                string error = "Error eliminando el tipo de prenda, El id no coincide con ningun dato";
                return RedirectToAction("Index", new { error = error });
            }
            _context.Tipo_Prenda.Remove(tipo_prenda);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }   

    }
}
