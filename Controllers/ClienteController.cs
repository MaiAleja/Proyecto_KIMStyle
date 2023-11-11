using KIM_Style.Data;
using KIM_Style.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace KIM_Style.Controllers
{
    public class ClienteController : Controller
    {
        private readonly KIM_StyleContext _context;
        private readonly VerificacionesServicio _verServicio;
        public ClienteController(KIM_StyleContext context)
        {
            this._context = context;
            this._verServicio = new VerificacionesServicio(_context);
        }

        [Authorize(Roles = "Cliente,Admin")]
        public IActionResult Index()
        {
            actualizarCalificaciones();
            List<Tipo_Talla> listaTallas = _context.Tipo_Talla.ToList();
            ViewBag.listaTallas = listaTallas;
            List<Tipo_Marca> listaMarcas = _context.Tipo_Marca.ToList();
            ViewBag.listaMarcas = listaMarcas;
            List<Color> listaColores = _context.Color.ToList();
            ViewBag.listaColores = listaColores;
            List<Tipo_Prenda> listaTipoPrendas = _context.Tipo_Prenda.ToList();
            ViewBag.listaTipoPrendas = listaTipoPrendas;
            List<Producto> listaProductos = _context.Producto
                .Include(u => u.Color)
                .Include(u => u.Tipo_Prenda)
                .Include(u => u.Tipo_Marca)
                .Include(u => u.Tipo_Talla)
                .Where(p => p.cantidad > 0)
                .ToList();
            if (TempData["tipoFiltro"] != null)
            {
                listaProductos = Filtrar_Productos(listaProductos);
            }
            ViewBag.listaProductos = listaProductos;
            if (TempData["CompraRealizada"] != null)
            {
                ViewBag.huboCompra = TempData["CompraRealizada"];
            }
            else
            {
                ViewBag.huboCompra = false;
            }

            int id_usuario = int.Parse(User.FindFirst(ClaimTypes.Name).Value.ToString());
            Usuario usuario = _context.Usuario.FirstOrDefault(u => u.cedula == id_usuario);
            ViewBag.usuario = usuario;

            return View();
        }

        [Authorize(Roles = "Cliente,Admin")]
        private void actualizarCalificaciones()
        {
            List<Producto> productos = _context.Producto.ToList();
            foreach (Producto producto in productos)
            {
                double promedio;
                List<Calificacion> calificaciones = _context.Calificacion.Where(c => c.cod_producto == producto.cod_producto).ToList();
                if(calificaciones != null && calificaciones.Count > 0)
                {
                    promedio = calificaciones.Average(c => c.calificacion);
                    int promedioRedondeado = (int)Math.Ceiling(promedio);
                    producto.calificacion_promedio = promedioRedondeado;
                }
                else
                {
                    producto.calificacion_promedio = 0;
                }
            }
            _context.SaveChanges();

        }

        [Authorize(Roles = "Cliente,Admin")]
        private List<Producto> Filtrar_Productos(List<Producto> listaProductos)
        {
            int id = 0;
            if (TempData["idFiltro"] != null)
            {
                id = int.Parse(TempData["idFiltro"].ToString());
            }
            if (TempData["tipoFiltro"].Equals("Marca"))
            {
                return listaProductos.Where(p => p.marca == id).ToList();
            }
            if (TempData["tipoFiltro"].Equals("Color"))
            {
                return listaProductos.Where(p => p.id_color == id).ToList();
            }
            if (TempData["tipoFiltro"].Equals("tipoP"))
            {
                return listaProductos.Where(p => p.id_tipo_prenda == id).ToList();
            }
            if (TempData["tipoFiltro"].Equals("Calificacion"))
            {
                return listaProductos.OrderByDescending(p => p.calificacion_promedio).ToList();
            }
            return listaProductos;
        }

        [Authorize(Roles = "Cliente,Admin")]
        [HttpPost]
        public IActionResult AgregarAlCarrito (int cod_producto)
        {
            int id_usuario = int.Parse(User.FindFirst(ClaimTypes.Name).Value.ToString());
            Factura ventaActiva = _context.Factura.FirstOrDefault(f => f.id_usuario == id_usuario && f.estado_venta.Equals("enCarrito"));
            if(ventaActiva == null && cod_producto == -1) {
                return PartialView("_CarritoCompra");
            }
            if (ventaActiva == null)
            {
                _context.Factura.Add( 
                    new Factura
                    {
                         id_usuario = id_usuario,
                         estado_venta = "enCarrito",
                         fecha = DateTime.Now,
                         tipo_transaccion = "carrito",
                         valor_total = 0,
                    }
                );
                _context.SaveChanges();
            }
            ventaActiva = _context.Factura.FirstOrDefault(f => f.id_usuario == id_usuario && f.estado_venta.Equals("enCarrito"));
            if(cod_producto != -1)
            {
                //Consulta para saber si ya esta en el carrito ese producto
                Detalle_Factura Detalle_producto = _context.Detalle_Factura.FirstOrDefault(d => d.cod_producto == cod_producto && d.id_factura == ventaActiva.id_factura);
                //Si no hay un detalle factura en esa factura activa que tenga el codigo del producto
                //seleccionado, entonces no esta todavia ese producto en el carrito y
                //hay que agregarlo
                if (Detalle_producto == null)
                {
                    _context.Detalle_Factura.Add(
                        new Detalle_Factura
                        {
                            cantidad = 0,
                            id_factura = ventaActiva.id_factura,
                            cod_producto = cod_producto,
                            total_producto = 0,
                        }
                    );
                    _context.SaveChanges();
                }
            }
            
            List<Detalle_Factura> listaDetallesFactura = _context.Detalle_Factura
                .Where(d => d.id_factura == ventaActiva.id_factura)
                .ToList();
            List<Producto> listaProductosCarrito = new List<Producto>();
            foreach (Detalle_Factura detalle_factura in listaDetallesFactura)
            {
                Producto producto = _context.Producto
                .Include(u => u.Color)
                .Include(u => u.Tipo_Prenda)
                .Include(u => u.Tipo_Marca)
                .Include(u => u.Tipo_Talla)
                .FirstOrDefault(p => p.cod_producto == detalle_factura.cod_producto);
                listaProductosCarrito.Add(producto);
            }
            ViewBag.listaProductosCarrito = listaProductosCarrito;
            return PartialView("_CarritoCompra");
        }

        [Authorize(Roles = "Cliente,Admin")]
        public IActionResult Compra()
        {
            int id_usuario = int.Parse(User.FindFirst(ClaimTypes.Name).Value.ToString());
            Factura ventaActiva = _context.Factura.FirstOrDefault(f => f.id_usuario == id_usuario && f.estado_venta.Equals("enCarrito"));
            if (ventaActiva == null)
            {
                return RedirectToAction("Index");
            }
            List<Detalle_Factura> listaDetallesFactura = _context.Detalle_Factura
                .Include(d => d.Producto)
                .Where(d => d.id_factura == ventaActiva.id_factura)
                .ToList();
            if(listaDetallesFactura == null || listaDetallesFactura.Count == 0)
            {
                return RedirectToAction("Index");
            }
            List<Producto> listaProductosCarrito = new List<Producto>();

            //dato para el total de la compra
            double totalCompra = 0;
            
                foreach (Detalle_Factura detalle_factura in listaDetallesFactura)
            {
                detalle_factura.cantidad = 1;
                detalle_factura.total_producto = detalle_factura.Producto.valor * detalle_factura.cantidad;
                _context.SaveChanges();
                totalCompra += detalle_factura.total_producto;
                Producto producto = _context.Producto
                .Include(u => u.Color)
                .Include(u => u.Tipo_Prenda)
                .Include(u => u.Tipo_Marca)
                .Include(u => u.Tipo_Talla)
                .FirstOrDefault(p => p.cod_producto == detalle_factura.cod_producto);
                listaProductosCarrito.Add(producto);
            }
            ventaActiva.valor_total = totalCompra;
            _context.SaveChanges();
            ViewBag.totalCompra = totalCompra;
            ViewBag.listaProductosCarrito = listaProductosCarrito;

            return View();
        }

        [Authorize(Roles = "Cliente,Admin")]
        public IActionResult ActualizarTotalCompra(int cod_producto, int cantidad)
        {
            double totalCompra = 0;
            int id_usuario = int.Parse(User.FindFirst(ClaimTypes.Name).Value);
            Factura ventaActiva = _context.Factura.FirstOrDefault(f => f.id_usuario == id_usuario && f.estado_venta.Equals("enCarrito"));
            List<Detalle_Factura> listaDetallesFactura = _context.Detalle_Factura
                .Include(d => d.Producto)
                .Where(d => d.id_factura == ventaActiva.id_factura)
                .ToList();
            foreach (Detalle_Factura detalle_factura in listaDetallesFactura)
            {
                if(detalle_factura.cod_producto == cod_producto)
                {
                    detalle_factura.cantidad = cantidad;
                    detalle_factura.total_producto = detalle_factura.Producto.valor * detalle_factura.cantidad;
                }
                _context.SaveChanges();
                totalCompra += detalle_factura.total_producto;
            }
            ventaActiva.valor_total = totalCompra;
            _context.SaveChanges();
            return Json(new { totalCompra });
        }

        [Authorize(Roles = "Cliente,Admin")]
        public IActionResult Eliminar_Producto(int cod_producto)
        {
            string correo = User.FindFirst(ClaimTypes.Name).Value;
            int id_usuario = _context.Usuario.FirstOrDefault(u => u.correo == correo).cedula;
            Factura ventaActiva = _context.Factura.FirstOrDefault(f => f.id_usuario == id_usuario && f.estado_venta.Equals("enCarrito"));
            Detalle_Factura detalle_Factura = _context.Detalle_Factura.FirstOrDefault(d => d.id_factura == ventaActiva.id_factura && d.cod_producto == cod_producto);
            _context.Detalle_Factura.Remove(detalle_Factura);
            _context.SaveChanges();
            return RedirectToAction("Compra", "Cliente");
        }

        [Authorize(Roles = "Cliente,Admin")]
        public IActionResult Finalizar_Compra()
        {
            int id_usuario = int.Parse(User.FindFirst(ClaimTypes.Name).Value.ToString());
            Factura ventaActiva = _context.Factura.FirstOrDefault(f => f.id_usuario == id_usuario && f.estado_venta.Equals("enCarrito"));
            ventaActiva.estado_venta = "Finalizado";
            ventaActiva.fecha = DateTime.Now;
            ventaActiva.tipo_transaccion = "Pago Virtual";
            _context.SaveChanges();
            restarCantidadEnProductos(ventaActiva);
            TempData["CompraRealizada"] = true;
            return RedirectToAction("Index", "Cliente");
        }

        [Authorize(Roles = "Cliente,Admin")]
        private void restarCantidadEnProductos(Factura venta)
        {
            List<Detalle_Factura> listaDetallesFactura = _context.Detalle_Factura
                .Include(d => d.Producto)
                .Where(d => d.id_factura == venta.id_factura)
                .ToList();
            foreach (Detalle_Factura detalle_factura in listaDetallesFactura)
            {
                Producto producto = detalle_factura.Producto;
                producto.cantidad = producto.cantidad - detalle_factura.cantidad;       
            }
            _context.SaveChanges();
        }

        [Authorize(Roles = "Cliente,Admin")]
        public IActionResult Filtrar_Productos(string tipoFiltro, int idFiltro)
        {
            TempData["tipoFiltro"] = tipoFiltro;
            TempData["idFiltro"] = idFiltro;
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Cliente,Admin")]
        [HttpPost]
        public ActionResult CalificarProducto(int cod_producto, int valor)
        {

            int id_usuario = int.Parse(User.FindFirst(ClaimTypes.Name).Value.ToString());
            Calificacion calificacion = _context.Calificacion.FirstOrDefault(c => c.ced_usuario == id_usuario && c.cod_producto == cod_producto);
            Producto producto = _context.Producto.FirstOrDefault(p => p.cod_producto == cod_producto);

            if (calificacion == null)
            {
                if (valor == -1)
                {
                    return PartialView("_calificacion-usuario", producto);
                }
                _context.Calificacion.Add(
                    new Calificacion
                    {
                        ced_usuario = id_usuario,
                        cod_producto = cod_producto,
                        calificacion = valor
                    }
                    );
                _context.SaveChanges();
                calificacion = _context.Calificacion.FirstOrDefault(c => c.ced_usuario == id_usuario && c.cod_producto == cod_producto);
            }
            else
            {
                if (valor == -1)
                {
                    ViewBag.calificacion = calificacion.calificacion;
                    return PartialView("_calificacion-usuario", producto);
                }
                calificacion.calificacion = valor;
                _context.SaveChanges();
            }

            ViewBag.calificacion = calificacion.calificacion; // Actualiza la calificación en ViewBag
            return PartialView("_calificacion-usuario",producto);
        }

        [Authorize(Roles = "Cliente,Admin")]
        public IActionResult Actualizar_Datos(Usuario usuario,string error)
        {
            ViewBag.error = error;
            int cedula = int.Parse(User.FindFirst(ClaimTypes.Name).Value.ToString());
            usuario = _context.Usuario.FirstOrDefault(u => u.cedula ==  cedula);
            return View(usuario);
        }

        [Authorize(Roles = "Cliente,Admin")]
        [HttpPost]
        public IActionResult ActualizarDatos(Usuario usuario, string conservarContrasena, IFormFile imagenPerfil)
        {
            int cedula = int.Parse(User.FindFirst(ClaimTypes.Name).Value.ToString());
            Usuario usuarioBD = _context.Usuario.FirstOrDefault(u => u.cedula == cedula);
            usuario.correo = usuarioBD.correo;
            string error = _verServicio.verificacionUsuario(usuario, true, conservarContrasena);
            if (!string.IsNullOrEmpty(error))
            {
                return RedirectToAction("Actualizar_Datos", "Cliente", new { error = error });
            }
            
            if(imagenPerfil != null)
            {
                usuario.FotoPerfil = _verServicio.ConvertirAByteArray(imagenPerfil);
            }
            if (usuarioBD.cedula != usuario.cedula)
            {
                if (_context.Usuario.FirstOrDefault(u => u.cedula == usuario.cedula) != null)
                {
                    error = "Ya existe un usuario con esa cedula";
                    return RedirectToAction("Agregar_Empleado", "Admin", new { error = error, accion = "Actualizar", metodo = "Actualizar_Empleado" });
                }
                if (conservarContrasena != null)
                {
                    usuario.constrasena = _verServicio.EncriptarContrasena(usuarioBD.constrasena);
                }
                usuario.rol_usuario = usuarioBD.rol_usuario;
                usuario.correo = usuarioBD.correo;
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
                usuarioBD.FotoPerfil = usuario.FotoPerfil;
                _context.SaveChanges();
            }
            usuarioBD = _context.Usuario.Include(u => u.Rol).FirstOrDefault(u => u.cedula == cedula);
            HttpContext.SignOutAsync("KimStyleCookie");
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuarioBD.cedula.ToString()),
                    new Claim(ClaimTypes.Role, usuarioBD.Rol.nombre_rol)
                };

            var identity = new ClaimsIdentity(claims, "KimStyleAutenticacion");
            var principal = new ClaimsPrincipal(identity);
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true // Establece IsPersistent en true para recordar la sesión
            };

            HttpContext.SignInAsync("KimStyleCookie", principal, authenticationProperties);

            return RedirectToAction("Index", "Cliente");
        }

    }
}
