using KIM_Style.Data;
using KIM_Style.Models;
using KIM_Style.Models.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace KIM_Style.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly KIM_StyleContext _context;
        private readonly VerificacionesServicio _verServicio;
        public HomeController(ILogger<HomeController> logger, KIM_StyleContext context, IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            _verServicio = new VerificacionesServicio(context, emailService);
        }

        public IActionResult Index(string error)
        {
            if (User.Identity.IsAuthenticated) //verificacion usuario log
            {
                string rol = User.FindFirst(ClaimTypes.Role).Value.ToString();
                if (rol.Equals("Empleado"))
                {
                    return RedirectToAction("Index","Empleado");
                }
                if (rol.Equals("Cliente"))
                {
                    return RedirectToAction("Index","Cliente");
                }
                if (rol.Equals("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            ViewBag.error = error;
            return View();
        }

        public IActionResult Registro(string error)
        {
            if (User.Identity.IsAuthenticated)
            {
                string rol = User.FindFirst(ClaimTypes.Role).Value.ToString();
                if (rol.Equals("Empleado"))
                {
                    return RedirectToAction("Index", "Empleado");
                }
                if (rol.Equals("Cliente"))
                {
                    return RedirectToAction("Index", "Cliente");
                }
                if (rol.Equals("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            ViewBag.error = error;
            Usuario usuario = new Usuario();
            if (TempData["usuario"] != null)
            {
                usuario = JsonConvert.DeserializeObject<Usuario>(TempData["usuario"].ToString());
            }
            return View(usuario);
        }

        public IActionResult Registrar(Usuario usuario)
        {
            TempData["usuario"] = JsonConvert.SerializeObject(usuario);
            string error = _verServicio.verificacionUsuario(usuario);
            if (string.IsNullOrEmpty(error))
            {
                usuario.rol_usuario = 3;
                TempData["usuario"] = null;
                usuario.constrasena = _verServicio.EncriptarContrasena(usuario.constrasena);
                _context.Usuario.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Registro", new { error = error });
        }

        public IActionResult Login(string correo, string contrasena)
        {
            string error = _verServicio.VerificarLogin(correo, contrasena);
            Usuario us = _context.Usuario.Include(u => u.Rol).FirstOrDefault(u => u.correo == correo);
            if(string.IsNullOrEmpty(error))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, us.cedula.ToString()),
                    new Claim(ClaimTypes.Role, us.Rol.nombre_rol)
                };

                var identity = new ClaimsIdentity(claims, "KimStyleAutenticacion");
                var principal = new ClaimsPrincipal(identity);
                var authenticationProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Establece IsPersistent en true para recordar la sesión
                };

                HttpContext.SignInAsync("KimStyleCookie", principal,authenticationProperties);
                return RedirectToAction("Index","Home");
            }
            return RedirectToAction("Index", new { error = error });
        }

        public IActionResult Cerrar_Sesion()
        {
            HttpContext.SignOutAsync("KimStyleCookie");
            return RedirectToAction("Index","Home");
        }

        public IActionResult Recuperacion(string error, string correo)
        {
            ViewBag.error = error;
            if(correo != null)
            {
                ViewBag.huboEnvio = true;
                ViewBag.correo = correo;
            }
            if (TempData["confirmacion"] != null)
            {
                ViewBag.correo = correo;
                ViewBag.confirmacion = true;
            }
            return View();
        }

        public IActionResult Envio_Codigo(string correo)
        {
            if(string.IsNullOrEmpty(correo))
            {
                return RedirectToAction("Recuperacion", new { error = "Correo vacío" });
            } 
            if (!Regex.IsMatch(correo, @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$"))
            {
                return RedirectToAction("Recuperacion", new { error = "Correo mal digitado" });
            }
            if (_context.Usuario.FirstOrDefault(u => u.correo == correo) == null)
            {
                return RedirectToAction("Recuperacion", new { error = "Correo no registrado en la pagina" });
            }

            int codigo = _verServicio.EnviarCorreo(correo);
            TempData["codigo"] = codigo;

            return RedirectToAction("Recuperacion", new { correo = correo });
        }

        public IActionResult Confirmar_Codigo(string correo,int codigo)
        {
            int cod = int.Parse(TempData["codigo"].ToString());
            if(cod != codigo)
            {
                return RedirectToAction("Recuperacion", new { error = "El codigo no concuerda con el enviado" });
            }
            TempData["confirmacion"] = "Correcto";
            return RedirectToAction("Recuperacion", new { correo = correo });
        }

        public IActionResult Cambiar_Contraseña(string correo, string contrasena)
        {
            if (_verServicio.ValidarContrasena(contrasena))
            {
                Usuario usuario = _context.Usuario.FirstOrDefault(u => u.correo == correo);
                if(usuario != null)
                {
                    usuario.constrasena = _verServicio.EncriptarContrasena(contrasena);
                    _context.SaveChanges();
                    return RedirectToAction("Index", new { error = "Cambio de Contraseña Existoso" });
                }
            }
            return RedirectToAction("Recuperacion", new { correo = correo});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}