using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        // Conexión a la base de datos.
        string conexion = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=TFC;Integrated Security=True";
        static TFCEntities db = new TFCEntities();

        // GET: Admin

        /// <summary>
        /// Método correspondiente al index de la vista Admin. Carga una serie de ViewBags para el dashboard.
        /// </summary>
        /// <param name="source">Indica si se llega hasta el método a partir de un botón.</param>
        /// <returns>Vista Index.</returns>
        public ActionResult Index(string source)
        {
            /*
             * ViewBags por defecto para la página.
             * Estos se usan para pasar datos entre el controlador y la vista.
             */
            ViewBag.renderBelow = "true";
            ViewBag.PermisoAdmin = false;

            // Se comprueba que el usuario sea administrador.
            if (Session["usuario"] != null)
            {
                string user = Session["usuario"].ToString();
                Usuarios usuario = db.Usuarios.Where(x => x.Usuario == user).First();

                if (usuario.Administrador == 1)
                    ViewBag.PermisoAdmin = true;
            }

            /*
             * Diferentes casos de redirección dependiendo de las sesiones.
             * Si el usuario no está logueado y no hay 'source', se redirige al Home.
             * Si el usuario no está logueado pero hay 'source', se devuelve un mensaje JSON de error.
             * Si el usuario está logueado y hay 'source', se devuelve un mensaje JSON de éxito.
             */
            if (Session["usuario"] == null && string.IsNullOrEmpty(source))
            {
                return RedirectToAction("Index", "Home");
            }
            else if (Session["usuario"] == null && !string.IsNullOrEmpty(source))
            {
                return Json(new { success = false, message = "Debes ser un usuario administrador para acceder al área de administración." });
            }
            else if (Session["usuario"] != null && !string.IsNullOrEmpty(source))
            {
                return Json(new { success = true, url = Url.Action("Index", "Admin") });
            }

            /*
             * Se obtienen diferentes datos para cargar el dashboard.
             * Por ejemplo, el total de usuarios no administradores.
             */
            ViewBag.TotalUsuarios = db.Usuarios.Count(u => u.Administrador == null);

            // Fecha de hoy y fecha de hace 3 meses.
            var hoy = DateTime.Today;
            var ult3meses = hoy.AddMonths(-3);

            // Usuarios registrados en los últimos 3 meses, agrupados por año y mes.
            var registrosUltimos3Meses = db.Usuarios.Where(u => u.FechaRegistro >= ult3meses && u.Administrador == null).GroupBy(u => new { u.FechaRegistro.Value.Year, u.FechaRegistro.Value.Month })
                .Select(g => new
                {
                    anio = g.Key.Year,
                    mes = g.Key.Month,
                    count = g.Count()
                }).OrderBy(g => g.anio).ThenBy(g => g.mes).ToList();

            ViewBag.RegistrosUltimos3Meses = registrosUltimos3Meses;

            // Promedio de stock por usuario no administrador.
            var mediaStockPorUsuario = db.Inventario.GroupBy(i => i.Cliente)
            .Select(g => new
            {
                cliente = g.Key,
                stocktotal = g.Sum(i => i.Cantidad)
            }).Where(g => db.Usuarios.Any(u => u.Usuario == g.cliente && u.Administrador == null)).Average(g => g.stocktotal);

            ViewBag.MediaStockPorUsuario = mediaStockPorUsuario;

            // Precio medio de los artículos de los usuarios no administradores.
            var precioMedioArticulos = db.Inventario.Where(i => db.Usuarios.Any(u => u.Usuario == i.Cliente && u.Administrador == null)).Average(i => (double?)i.Precio) ?? 0.0;

            ViewBag.PrecioMedioArticulos = precioMedioArticulos;

            return View();
        }

        /// <summary>
        /// Método correspondiente a la página Gestion.cshtml de la vista Admin.
        /// </summary>
        /// <returns>Vista Gestión.</returns>
        public ActionResult Gestion()
        {
            /*
             * ViewBags por defecto para la página.
             */
            ViewBag.renderBelow = "true";

            // Se comprueban los permisos para evitar accesos no autorizados.
            if (Session["usuario"] != null)
            {
                string user = Session["usuario"].ToString();
                Usuarios usuario = db.Usuarios.Where(x => x.Usuario == user).First();

                if (usuario.Administrador == 0)
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.PermisoAdmin = true;

            // Se devuelve una lista con todos los identificadores de Zapatilla para manejarlos en la vista.
            var zapas = new Zapatillas();
            var skus = db.Zapatillas.Select(x => x.SKU).ToList();
            ViewBag.SKUs = skus;

            return View(zapas);
        }

        /// <summary>
        /// Añade un nuevo registro a la base de datos de Zapatillas.
        /// </summary>
        /// <param name="z">Objeto Zapatillas a añadir.</param>
        /// <returns>JSON indicando el resultado de la operación.</returns>
        public ActionResult AddZapas(Zapatillas z)
        {
            // Convierte los números en palabras del modelo a dígitos.
            z.Modelo = ConvertNumbersToDigits(z.Modelo);

            // Comprueba si ya existe una zapatilla con el mismo SKU.
            var aux = db.Zapatillas.Where(x => x.SKU == z.SKU).FirstOrDefault();

            if (aux != null)
            {
                // Si ya existe, devuelve un mensaje de error.
                return Json(new { success = false, message = "El artículo ya se encuentra en la base de datos." });
            }
            else
            {
                // Si no existe, se añade y se guarda en la base de datos.
                db.Zapatillas.Add(z);
                db.SaveChanges();

                return Json(new { success = true });
            }
        }

        /// <summary>
        /// Edita un registro existente en la base de datos de Zapatillas.
        /// </summary>
        /// <param name="z">Objeto Zapatillas con los nuevos datos.</param>
        /// <returns>JSON indicando el resultado de la operación.</returns>
        public ActionResult EditZapas(Zapatillas z)
        {
            // Busca la zapatilla existente por SKU.
            var aux = db.Zapatillas.Where(x => x.SKU == z.SKU).FirstOrDefault();

            if (aux.CompareTo(z) == 0)
                return Json(new { success = false, message = "No has cambiado ningún dato." });
            else
            {
                // Actualiza los datos de la zapatilla.
                aux.Nombre = z.Nombre;
                aux.Marca = z.Marca;
                aux.Modelo = z.Modelo;
                aux.FechaLanzamiento = z.FechaLanzamiento;
                aux.Descripcion = z.Descripcion;

                db.SaveChanges();
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Obtiene los datos de una zapatilla específica a partir de un sku.
        /// </summary>
        /// <param name="sku">SKU de la zapatilla.</param>
        /// <returns>JSON con los datos de la zapatilla.</returns>
        public ActionResult ObtenerDatosZapa(string sku)
        {
            // Busca la zapatilla por SKU.
            var aux = db.Zapatillas.Where(x => x.SKU == sku).First();
            var datos = new ZapatillasDisplay
            {
                SKU = aux.SKU,
                Nombre = aux.Nombre,
                Marca = aux.Marca,
                Modelo = aux.Modelo,
                FechaLanzamiento = aux.FechaLanzamiento,
                Descripcion = aux.Descripcion != null ? aux.Descripcion : "El artículo no tiene descripción"
            };

            // Se devuelve una respuesta en función de los resultados.
            if (datos != null)
            {
                return Json(new { success = true, datos });
            }
            else
            {
                return Json(new { success = false, message = "No se han encontrado datos." });
            }
        }

        /// <summary>
        /// Convierte palabras que representan números a dígitos.
        /// </summary>
        /// <param name="input">Cadena de texto a convertir.</param>
        /// <returns>Cadena de texto con los números convertidos a dígitos.</returns>
        private string ConvertNumbersToDigits(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Diccionario de palabras a números.
            Dictionary<string, string> nums = new Dictionary<string, string>
            {
                { "zero", "0" },
                { "one", "1" },
                { "two", "2" },
                { "three", "3" },
                { "four", "4" },
                { "five", "5" },
                { "six", "6" },
                { "seven", "7" },
                { "eight", "8" },
                { "nine", "9" }
            };

            // Reemplaza las palabras en el input con sus números correspondientes.
            foreach (var num in nums)
            {
                input = System.Text.RegularExpressions.Regex.Replace(input, @"\b" + num.Key + @"\b", num.Value, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return input;
        }
    }
}
