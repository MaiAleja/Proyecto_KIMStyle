using KIM_Style.Data;
using KIM_Style.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace KIM_Style.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly KIM_StyleContext _context;
        private readonly VerificacionesServicio _verServicio;
        public EmpleadoController(KIM_StyleContext context)
        {
            _context = context;
            this._verServicio = new VerificacionesServicio(_context);
        }

        [Authorize(Roles = "Empleado,Admin")]
        public IActionResult Index(string error)
        {
            List<Producto> listaProductos = _context.Producto
                .Include(u => u.Color)
                .Include(u => u.Tipo_Prenda)
                .Include(u => u.Tipo_Marca)
                .Include(u => u.Tipo_Talla)
                .ToList();
            ViewBag.listaProductos = listaProductos;
            return View();
        }

        [Authorize(Roles = "Empleado,Admin")]
        public IActionResult Registro_Venta()
        {
            List<Factura> listaFacturas = _context.Factura.ToList();
            ViewBag.listaFacturas = listaFacturas;
            return View();
        }

        [Authorize(Roles = "Empleado,Admin")]
        public IActionResult Agregar_prenda(string error, string accion = "Agregar", string metodo = "AgregarPrenda")
        {
            Producto producto = new Producto();
            //Pasar lista de Tallas de la BD a la vista
            List<Tipo_Talla> listaTallas = _context.Tipo_Talla.ToList();
            ViewBag.listaTallas = listaTallas;
            //Pasar lista de Marcas de la BD a la vista
            List<Tipo_Marca> listaMarcas = _context.Tipo_Marca.ToList();
            ViewBag.listaMarcas = listaMarcas;
            //Pasar lista de Colores de la BD a la vista
            List<Color> listaColores = _context.Color.ToList();
            ViewBag.listaColores = listaColores;
            //Pasar lista de Tipo_Prenda de la BD a la vista
            List<Tipo_Prenda> listaTipoPrendas = _context.Tipo_Prenda.ToList();
            ViewBag.listaTipoPrendas = listaTipoPrendas;
            ViewBag.error = error;
            ViewBag.accion = accion;
            ViewBag.metodo = metodo;
            if (TempData["producto"] != null)
            {
                producto = JsonConvert.DeserializeObject<Producto>(TempData["producto"].ToString());
            }
            return View(producto);
        }

        [Authorize(Roles = "Empleado,Admin")]
        [HttpPost]
        public IActionResult AgregarPrenda(Producto producto, IFormFile fotoProductoArchivo)
        {
            if (fotoProductoArchivo != null)
            {
                producto.Imagen = _verServicio.ConvertirAByteArray(fotoProductoArchivo);

            }
            TempData["producto"] = JsonConvert.SerializeObject(producto);
            string error = _verServicio.VerificarProducto(producto);
            if(error != null)
            {
                return RedirectToAction("Agregar_prenda", new { error = error });
            }
            producto.material = producto.material.Trim();
            producto.temporada = producto.temporada.Trim();
            _context.Producto.Add(
                new Producto {
                    temporada = producto.temporada,
                    material = producto.material,
                    genero = producto.genero,
                    cantidad = producto.cantidad,
                    valor = producto.valor,
                    id_color = producto.id_color,
                    id_tipo_prenda = producto.id_tipo_prenda,
                    Imagen = producto.Imagen,
                    talla = producto.talla,
                    marca = producto.marca,
                }
                );
            _context.SaveChanges();
            TempData["producto"] = null;
            return RedirectToAction("Index", "Empleado");
        }

        [Authorize(Roles = "Empleado,Admin")]
        [HttpPost]
        public IActionResult Modificar_Producto(int cod_producto, string accion)
        {
            Producto producto = _context.Producto.FirstOrDefault(u => u.cod_producto == cod_producto);
            if (producto == null)
            {
                return RedirectToAction("Index", new { error = "Ese Codigo No Está Registrado" });
            }
            if (accion == "Actualizar")
            {
                TempData["cod_producto"] = producto.cod_producto;
                if(producto.Imagen != null) producto.Imagen = Encoding.UTF8.GetBytes("someting");
                TempData["producto"] = JsonConvert.SerializeObject(producto);
                return RedirectToAction("Agregar_prenda", "Empleado", new { accion = "Actualizar", metodo = "Actualizar_Prenda" });
            }
            if (accion == "Eliminar")
            {
                return RedirectToAction("Eliminar_Prenda", "Empleado", new { cod_producto = cod_producto});
            }
            return RedirectToAction("Index", new { error = "Accion aparentemente no registrada" });
        }

        [Authorize(Roles = "Empleado,Admin")]
        [HttpPost]
        public IActionResult Actualizar_Prenda(Producto producto, IFormFile fotoProductoArchivo)
        {
            TempData["producto"] = JsonConvert.SerializeObject(producto);
            string error = _verServicio.VerificarProducto(producto);
            if (!string.IsNullOrEmpty(error))
            {
                return RedirectToAction("Agregar_Empleado", "Admin", new { error = error, accion = "Actualizar", metodo = "Actualizar_Empleado" });
            }
            if(fotoProductoArchivo != null)
            {
                producto.Imagen = _verServicio.ConvertirAByteArray(fotoProductoArchivo);
            }
            Producto productoBD = _context.Producto.FirstOrDefault(u => u.cod_producto == int.Parse(TempData["cod_producto"].ToString()));
            TempData["cod_producto"] = null;
            productoBD.temporada = producto.temporada;
            productoBD.material = producto.material;
            productoBD.genero = producto.genero;
            productoBD.cantidad = producto.cantidad;
            productoBD.valor = producto.valor;
            productoBD.id_color = producto.id_color;
            productoBD.id_tipo_prenda = producto.id_tipo_prenda;
            if(producto.Imagen != null) productoBD.Imagen = producto.Imagen;
            productoBD.talla = producto.talla;
            productoBD.marca = producto.marca;
            _context.SaveChanges();
            TempData["producto"] = null;
            return RedirectToAction("Index", "Empleado");
        }

        [Authorize(Roles = "Empleado,Admin")]
        public IActionResult Eliminar_Prenda(int cod_producto)
        {
            Producto producto = _context.Producto.FirstOrDefault(p => p.cod_producto == cod_producto);
            _context.Producto.Remove(producto);
            _context.SaveChanges();
            return RedirectToAction("Index", new { error = "Eliminado correctamente" });
        }

        [Authorize(Roles = "Empleado,Admin")]
        public IActionResult Agregar_Venta(string error)
        {
            ViewBag.error = error;
            if (TempData["id_usuario"] != null)
            {
                int id_usuario = int.Parse(TempData["id_usuario"].ToString());
                Factura factura = _context.Factura.FirstOrDefault(f => f.id_usuario == id_usuario && f.estado_venta.Equals("En_Facturacion"));
                if (factura == null) 
                {
                    _context.Factura.Add
                    (
                        new Factura 
                        {
                            id_usuario=id_usuario,
                            estado_venta= "En_Facturacion",
                            valor_total = 0,
                            fecha = DateTime.Now,
                            tipo_transaccion = "Compra En Local"
                        } 
                    );
                    _context.SaveChanges();
                    factura = _context.Factura.FirstOrDefault(f => f.id_usuario == id_usuario && f.estado_venta.Equals("En_Facturacion"));
                }
                ViewBag.venta = factura;
                List<Detalle_Factura> productosRegistrados = _context.Detalle_Factura
                    .Include(d => d.Producto)
                    .ThenInclude(p => p.Tipo_Marca)
                    .Include(d => d.Producto)
                    .ThenInclude(p => p.Tipo_Prenda)
                    .Where(d => d.id_factura == factura.id_factura)
                    .ToList();
                ViewBag.productosRegistrados = productosRegistrados;
            }
            return View();
        }

        [Authorize(Roles = "Empleado,Admin")]
        public IActionResult Comenzar_Venta(int id_usuario)
        {
            if(id_usuario != 0 && _context.Usuario.FirstOrDefault(u => u.cedula == id_usuario) != null)
            {
                TempData["id_usuario"] = id_usuario;
                return RedirectToAction("Agregar_Venta");
            }
            return RedirectToAction("Agregar_Venta",new { error = "Cedula de Usuario no Valida"});
        }

        [Authorize(Roles = "Empleado,Admin")]
        public IActionResult Agregar_Producto_Venta(int cod_producto, int cantidad, int id_factura, int id_usuario)
        {
            TempData["id_usuario"] = id_usuario;
            Producto producto = _context.Producto.FirstOrDefault(p => p.cod_producto == cod_producto);
            Factura factura = _context.Factura.FirstOrDefault(f => f.id_factura ==  id_factura);
            if(producto == null)
            {
                return RedirectToAction("Agregar_Venta", new { error = "Ningun Producto con ese ID" });
            }
            if(producto.cantidad < cantidad)
            {
                return RedirectToAction("Agregar_Venta", new { error = "Cantidad Insuficiente para este registro" });
            }
            Detalle_Factura detalle_Factura = _context.Detalle_Factura.FirstOrDefault(d => d.cod_producto == cod_producto && d.id_factura == id_factura);
            if(detalle_Factura == null)
            {
                _context.Detalle_Factura.Add
                (
                    new Detalle_Factura 
                    {
                        cantidad = cantidad,
                        total_producto = cantidad * producto.valor,
                        cod_producto = cod_producto,
                        id_factura = id_factura,
                    }
                );
                factura.valor_total += cantidad * producto.valor;
            }
            else
            {
                factura.valor_total -= detalle_Factura.cantidad * producto.valor;
                detalle_Factura.cantidad = cantidad;
                detalle_Factura.total_producto = cantidad * producto.valor;
                factura.valor_total += cantidad * producto.valor;
            }
            _context.SaveChanges();
            return RedirectToAction("Agregar_Venta");
        }

        [Authorize(Roles = "Empleado,Admin")]
        public IActionResult Eliminar_Producto_Venta(int id_detalle)
        {
            Detalle_Factura detalle = _context.Detalle_Factura.FirstOrDefault(d => d.id_detalle == id_detalle);
            Factura factura = _context.Factura.FirstOrDefault(f => f.id_factura == detalle.id_factura);
            TempData["id_usuario"] = factura.id_usuario;
            factura.valor_total -= detalle.total_producto;
            _context.Detalle_Factura.Remove(detalle);
            _context.SaveChanges();
            return RedirectToAction("Agregar_Venta");
        }

        [Authorize(Roles = "Empleado,Admin")]
        public IActionResult Eliminar_Factura(int id_factura)
        {
            Factura factura = _context.Factura.FirstOrDefault(f => f.id_factura == id_factura);
            _context.Remove(factura);
            _context.SaveChanges();
            return RedirectToAction("Agregar_Venta");
        }

        [Authorize(Roles = "Empleado,Admin")]
        public IActionResult Registrar_Factura(int id_factura)
        {
            Factura factura = _context.Factura.FirstOrDefault(f => f.id_factura == id_factura);
            restarCantidadEnProductos(factura);
            factura.estado_venta = "Finalizado";
            _context.SaveChanges();
            return RedirectToAction("Registro_Venta", "Empleado");
        }

        [Authorize(Roles = "Empleado,Admin")]
        private void restarCantidadEnProductos(Factura factura)
        {
            List<Detalle_Factura> listaDetallesFactura = _context.Detalle_Factura
                .Include(d => d.Producto)
                .Where(d => d.id_factura == factura.id_factura)
                .ToList();
            foreach (Detalle_Factura detalle_factura in listaDetallesFactura)
            {
                Producto producto = detalle_factura.Producto;
                producto.cantidad = producto.cantidad - detalle_factura.cantidad;
            }
            _context.SaveChanges();
        }
    }
}
