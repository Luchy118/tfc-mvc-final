using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using WebApplication1.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Globalization;
using static WebApplication1.Models.GeneralModel;
using Microsoft.Reporting.WebForms;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// Controlador para gestionar las acciones relacionadas con los usuarios.
    /// </summary>
    public class UsuariosController : Controller
    {
        // Conexión a la base de datos.
        string conexion = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=TFC;Integrated Security=True";
        static TFCEntities db = new TFCEntities();

        // Cliente HTTP estático para realizar solicitudes web
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Método para mostrar la vista principal de los usuarios.
        /// </summary>
        /// <param name="source">Origen de la solicitud.</param>
        /// <returns>ActionResult que representa la vista correspondiente.</returns>
        public ActionResult Index(string source)
        {
            // Se establece una variable en la vista para renderizar cierto contenido.
            ViewBag.renderBelow = "true";

            // Si hay una sesión de usuario activa
            if (Session["usuario"] != null)
            {
                // Se obtiene el nombre de usuario de la sesión.
                string user = Session["usuario"].ToString();
                // Se busca el usuario en la base de datos.
                Usuarios usuario = db.Usuarios.Where(x => x.Usuario == user).First();

                // Si el usuario es un administrador, se redirige a la página de administrador.
                if (usuario.Administrador == 1)
                {
                    ViewBag.PermisoAdmin = true;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.PermisoAdmin = false;
            }

            // Si no hay sesión de usuario y no hay un origen especificado
            if (Session["usuario"] == null && string.IsNullOrEmpty(source))
            {
                // Se redirige a la página principal.
                return RedirectToAction("Index", "Home");
            }
            // Si no hay sesión de usuario pero hay un origen especificado
            else if (Session["usuario"] == null && !string.IsNullOrEmpty(source))
            {
                // Se devuelve un JSON indicando que se debe iniciar sesión.
                return Json(new { success = false, message = "Debes iniciar sesión para acceder al área de usuarios." });
            }
            // Si hay sesión de usuario y hay un origen especificado
            else if (Session["usuario"] != null && !string.IsNullOrEmpty(source))
            {
                // Se devuelve un JSON con éxito y la URL para redirigir al área de usuarios.
                return Json(new { success = true, url = Url.Action("Index", "Usuarios") });
            }

            // Se muestra la vista principal de usuarios
            return View();
        }

        /// <summary>
        /// Método para obtener los datos del inventario del usuario actual.
        /// </summary>
        /// <returns>ActionResult con los datos del inventario en formato JSON.</returns>
        public ActionResult GetInventoryData()
        {
            // Se obtiene el nombre de usuario de la sesión.
            string cliente = Session["usuario"].ToString();

            // Se consultan los datos del inventario del usuario y se agrupan por SKU.
            var data = db.Inventario.Where(x => x.Cliente == cliente).GroupBy(x => x.SKU)
                .Join(db.Zapatillas, inv => inv.Key, zap => zap.SKU, (inv, zap) => new
                {
                    SKU = inv.Key,
                    zap.Nombre,
                    Cantidad = inv.Sum(x => x.Cantidad),
                    inv.FirstOrDefault().Precio
                })
                .ToList();

            // Si hay datos en el inventario
            if (data.Count > 0)
            {
                // Se devuelve un JSON con éxito y los datos del inventario.
                return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Se devuelve un JSON indicando que no hay datos en el inventario.
                return Json(new { success = false, data });
            }
        }

        /// <summary>
        /// Método asincrónico para obtener detalles de una fila de inventario.
        /// </summary>
        /// <param name="sku">SKU del producto.</param>
        /// <returns>ActionResult que representa la vista de detalles del producto.</returns>
        public async Task<ActionResult> InventoryRowDetails(string sku)
        {
            // Si no hay una sesión de usuario activa
            if (Session["usuario"] == null)
            {
                // Se redirige a la página principal.
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Se obtiene el nombre de usuario de la sesión.
                string user = Session["usuario"].ToString();
                // Se busca el usuario en la base de datos.
                Usuarios usuario = db.Usuarios.FirstOrDefault(x => x.Usuario == user);

                // Si el usuario no existe
                if (usuario == null)
                {
                    // Se devuelve un error indicando que el usuario no fue encontrado.
                    return HttpNotFound("Usuario no encontrado");
                }

                // Si el usuario es un administrador, se redirige a la página de administrador.
                if (usuario.Administrador == 1)
                {
                    ViewBag.PermisoAdmin = true;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.PermisoAdmin = false;
                }
            }

            // Se establece una variable en la vista para renderizar cierto contenido.
            ViewBag.renderBelow = "true";

            // Se busca el producto en el inventario.
            Inventario i = db.Inventario.FirstOrDefault(x => x.SKU.Equals(sku));
            Zapatillas z = db.Zapatillas.FirstOrDefault(x => x.SKU.Equals(sku));

            // Si el producto o la zapatilla no existen.
            if (i == null || z == null)
            {
                // Se devuelve un error indicando que el producto no fue encontrado.
                return HttpNotFound("Inventario o Zapatilla no encontrados");
            }

            // Se obtienen los datos de la zapatilla de una fuente externa de manera asincrónica.
            string datosZapatilla = await ObtenerDatosZapatilla(sku);
            JObject zapatillaJson = JObject.Parse(datosZapatilla);

            // Se extraen las tallas de la zapatilla
            var variants = zapatillaJson["hits"][0]["variants"];
            var tallas = variants.Select(v => (string)v["size"]);

            // Se obtiene el nombre y la URL de la imagen de la zapatilla.
            var nombre = db.Zapatillas.Where(x => x.SKU == sku).Select(x => x.Nombre).FirstOrDefault();
            ViewBag.Nombre = nombre;
            var urlImagen = db.Zapatillas.Where(x => x.SKU == sku).Select(x => x.Imagen).FirstOrDefault();
            ViewBag.UrlImagen = urlImagen;

            // Se calculan los precios de mercado en euros para las tallas disponibles.
            List<decimal> preciosDeMercadoEuros = new List<decimal>();
            decimal tasaCambioDolarEuro = 0.93m;
            foreach (var talla in tallas)
            {
                var precioTallaDolares = (decimal?)variants.FirstOrDefault(v => (string)v["size"] == talla)?["price"];
                if (precioTallaDolares != null)
                {
                    decimal precioTallaEuros = precioTallaDolares.Value * tasaCambioDolarEuro;
                    preciosDeMercadoEuros.Add(precioTallaEuros);
                }
            }

            // Se calcula el precio medio del mercado en euros.
            decimal mediaPrecioMercadoEuros = 0;
            if (preciosDeMercadoEuros.Any())
            {
                mediaPrecioMercadoEuros = preciosDeMercadoEuros.Average();
            }

            // Se establece la variable en la vista para mostrar el precio medio del mercado en euros.
            ViewBag.MediaPrecioMercadoEuros = mediaPrecioMercadoEuros;

            // Se obtienen los precios por talla de la zapatilla.
            Dictionary<decimal, decimal> precioPorTalla = ObtenerPreciosPorTalla(zapatillaJson);
            ViewData["PrecioPorTalla"] = precioPorTalla;

            // Se obtiene el precio del producto del inventario.
            decimal precio = (decimal)db.Inventario.Where(x => x.SKU == sku).Select(x => x.Precio).FirstOrDefault();
            ViewBag.Precio = precio;

            // Se obtienen las tallas disponibles en el inventario.
            var tallas2 = db.Inventario.Where(x => x.SKU.Equals(sku) && x.Cantidad != 0).Select(x => x.Talla).ToList().ConvertAll(x => Convert.ToDecimal(x));

            // Se crea una lista de elementos seleccionables para las tallas disponibles.
            List<SelectListItem> listaTallas = tallas2.Select(t => new SelectListItem
            {
                Value = t.ToString(),
                Text = t % 1 == 0 ? t.ToString("0") : t.ToString("0.0"),
                Selected = false
            }).ToList();

            // Se establece la lista de tallas en la vista.
            ViewData["Talla"] = new SelectList(listaTallas, "Value", "Text");

            // Se obtienen las tallas con stock disponible.
            var tallasConStock = new Dictionary<decimal, int>();
            var tallasOrdenadas = tallas2.Distinct().OrderBy(t => t).ToList();
            foreach (var talla in tallasOrdenadas)
            {
                if (!tallasConStock.ContainsKey(talla))
                {
                    int stock = (int)db.Inventario.Where(x => x.SKU == sku && x.Talla == talla).Sum(x => x.Cantidad);
                    if (stock != 0)
                    {
                        tallasConStock.Add(talla, stock);
                    }
                }
            }
            ViewBag.TallasConStock = tallasConStock;

            // Se obtiene la descripción de la zapatilla.
            var descripcion = string.IsNullOrEmpty(z.Descripcion) ? "El artículo no tiene descripción." : z.Descripcion;
            ViewBag.Descripcion = descripcion;

            // Se obtiene el enlace a StockX para la zapatilla.
            var stockxlink = (string)zapatillaJson["hits"][0]["link"];
            ViewBag.stockxlink = stockxlink;

            // Se muestra la vista de detalles del producto.
            return View(i);
        }

        /// <summary>
        /// Método para obtener los precios por talla de una zapatilla a partir de una respuesta JSON.
        /// </summary>
        /// <param name="jsonResponse">Respuesta JSON con los datos de la zapatilla.</param>
        /// <returns>Un diccionario que mapea las tallas europeas a los precios en euros.</returns>
        private Dictionary<decimal, decimal> ObtenerPreciosPorTalla(JObject jsonResponse)
        {
            // Tasa de cambio de dólares a euros
            decimal tasaDeCambio = 0.93m;
            // Diccionario para almacenar los precios por talla en dólares
            Dictionary<decimal, decimal> preciosPorTalla = new Dictionary<decimal, decimal>();
            // Diccionario para almacenar los precios por talla en euros
            Dictionary<decimal, decimal> preciosPorTallaEnEuros = new Dictionary<decimal, decimal>();

            // Lista de tallas en sistema US
            List<string> tallasUS = new List<string> { "4", "4.5", "5", "5.5", "6", "6.5", "7", "7.5", "8", "8.5", "9", "10", "10.5", "11", "11.5", "12", "12.5", "13" };
            // Diccionario para convertir tallas US a tallas europeas
            Dictionary<string, string> conversionTallas = new Dictionary<string, string>
            {
                { "4", "36m" },
                { "4.5", "36.5m" },
                { "5", "37m" },
                { "5.5", "38m" },
                { "6", "38.5m" },
                { "6.5", "39m" },
                { "7", "40m" },
                { "7.5", "40.5m" },
                { "8", "41m" },
                { "8.5", "42m" },
                { "9", "42.5m" },
                { "10", "43m" },
                { "10.5", "44m" },
                { "11", "44.5m" },
                { "11.5", "45m" },
                { "12", "45.5m" },
                { "12.5", "46m" },
                { "13", "47m" }
            };

            // Se recorren los hits de la respuesta JSON
            var hits = jsonResponse["hits"];
            foreach (var hit in hits)
            {
                var variants = hit["variants"];
                foreach (var variant in variants)
                {
                    string tallaStr = (string)variant["size"];

                    // Si la talla es en sistema US
                    if (tallasUS.Contains(tallaStr))
                    {
                        string tallaEuropea = conversionTallas[tallaStr];

                        decimal precio;
                        // Se intenta convertir el precio a decimal
                        if (decimal.TryParse((string)variant["price"], NumberStyles.Currency, CultureInfo.InvariantCulture, out precio))
                        {
                            // Se añade al diccionario de precios por talla en dólares
                            preciosPorTalla[decimal.Parse(tallaEuropea.Replace("m", ""), CultureInfo.InvariantCulture)] = precio;
                        }
                    }
                }
            }

            // Se convierten los precios a euros
            foreach (var aux in preciosPorTalla)
            {
                decimal precioEnDolares = aux.Value;
                decimal precioEnEuros = precioEnDolares * tasaDeCambio;
                preciosPorTallaEnEuros[aux.Key] = precioEnEuros;
            }

            // Se ordenan los precios por talla y se convierten a un diccionario ordenado
            preciosPorTallaEnEuros = preciosPorTallaEnEuros.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            return preciosPorTallaEnEuros;
        }

        /// <summary>
        /// Método para obtener datos de una zapatilla de una fuente externa de manera asincrónica.
        /// </summary>
        /// <param name="sku">SKU de la zapatilla.</param>
        /// <returns>Una tarea asincrónica que representa los datos de la zapatilla.</returns>
        public static async Task<string> ObtenerDatosZapatilla(string sku)
        {
            try
            {
                // Se crea una solicitud HTTP para obtener los datos de la zapatilla
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://the-sneaker-database-api-your-ultimate-sneaker-encyclopedia.p.rapidapi.com/search?query={sku}"),
                    Headers =
                    {
                        { "x-rapidapi-key", "79fcd3dd57msh35b829082e21612p1eaa4cjsn800aa5d34e7e" },
                        { "x-rapidapi-host", "the-sneaker-database-api-your-ultimate-sneaker-encyclopedia.p.rapidapi.com" },
                    },
                };

                // Se envía la solicitud HTTP de manera asincrónica y se obtiene la respuesta
                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    return body;
                }
            }
            catch (HttpRequestException e)
            {
                // Si hay un error en la solicitud HTTP, se muestra un mensaje en la consola
                Console.WriteLine($"Error al hacer la solicitud HTTP: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Método para obtener el stock de una zapatilla en una determinada talla.
        /// </summary>
        /// <param name="talla">Talla de la zapatilla.</param>
        /// <param name="sku">SKU de la zapatilla.</param>
        /// <returns>ActionResult que representa el stock de la zapatilla.</returns>
        public ActionResult ObtenerStock(decimal talla, string sku)
        {
            // Se obtiene el nombre de usuario de la sesión
            var usuario = Session["usuario"].ToString();
            // Se busca el stock de la zapatilla para el usuario y la talla especificados
            var stock = db.Inventario.Where(x => x.SKU == sku && x.Talla == talla && x.Cliente == usuario).FirstOrDefault(x => x.Talla == talla)?.Cantidad ?? 0;
            // Se devuelve el stock en formato JSON
            return Json(stock, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Método para obtener el precio de una zapatilla.
        /// </summary>
        /// <param name="sku">SKU de la zapatilla.</param>
        /// <returns>ActionResult que representa el precio de la zapatilla.</returns>
        public ActionResult ObtenerPrecio(string sku)
        {
            // Se busca el precio de la zapatilla en la base de datos
            var precio = db.Inventario.Where(x => x.SKU == sku).Select(x => x.Precio).FirstOrDefault();
            // Se devuelve el precio en formato JSON
            return Json(precio, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Método para añadir o editar el stock de una zapatilla en el inventario.
        /// </summary>
        /// <param name="Talla">Talla de la zapatilla.</param>
        /// <param name="Cantidad">Cantidad de stock.</param>
        /// <param name="sku">SKU de la zapatilla.</param>
        /// <returns>ActionResult que indica el éxito de la operación.</returns>
        public ActionResult AddOrEditInventoryStock(string Talla, string Cantidad, string sku)
        {
            // Conversión de los parámetros a los tipos correspondientes
            decimal auxTalla = Convert.ToDecimal(Talla);
            int auxCantidad = Convert.ToInt32(Cantidad);
            string usuario = Session["usuario"].ToString();
            // Se busca el artículo en el inventario
            Inventario i = db.Inventario.Where(x => x.SKU == sku && x.Talla == auxTalla && x.Cliente == usuario).FirstOrDefault();

            if (i == null)
            {
                // Si el artículo no existe en el inventario, se crea y se añade
                i = db.Inventario.Where(x => x.SKU == sku && x.Cliente == usuario).FirstOrDefault();
                i.Talla = auxTalla;
                i.Cantidad = auxCantidad;

                db.Inventario.Add(i);
                db.SaveChanges();

                return Json(new { success = true });
            }

            if (auxCantidad == i.Cantidad)
            {
                // Si la cantidad es la misma, no hay cambios
                return Json(new { success = false, message = "No hay cambios." });
            }
            else
            {
                // Si la cantidad es diferente, se actualiza el stock
                db.Inventario.Remove(i);
                db.SaveChanges();

                i.SKU = sku;
                i.Cliente = usuario;
                i.Cantidad = auxCantidad;
                db.Inventario.Add(i);
                db.SaveChanges();

                return Json(new { success = true });
            }
        }

        /// <summary>
        /// Método para añadir o editar el precio de un artículo en el inventario.
        /// </summary>
        /// <param name="precio">Precio del artículo.</param>
        /// <param name="sku">SKU del artículo.</param>
        /// <returns>ActionResult que indica el éxito de la operación.</returns>
        public ActionResult AddOrEditInventoryItemPrice(string precio, string sku)
        {
            int cont = 0;
            precio = precio.Replace(".", ",");
            decimal auxPrecio = Convert.ToDecimal(precio);

            foreach (var inv in db.Inventario.Where(x => x.SKU == sku))
            {
                if (inv != null)
                {
                    inv.Precio = Convert.ToDecimal(precio);
                    cont++;
                }
            }

            db.SaveChanges();

            if (cont != 0)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Ha ocurrido un error inesperado, por favor inténtelo de nuevo." });
            }
        }

        /// <summary>
        /// Método para eliminar un artículo del inventario.
        /// </summary>
        /// <param name="sku">SKU del artículo.</param>
        /// <returns>ActionResult que indica el éxito de la operación.</returns>
        public ActionResult DeleteInventoryItem(string sku)
        {
            // Se devuelve un mensaje de éxito
            return Json(new { success = true });
        }

        /// <summary>
        /// Método para obtener los SKU, nombres y descripciones de los artículos que no están en el inventario.
        /// </summary>
        /// <returns>ActionResult que representa los datos de los artículos.</returns>
        public ActionResult GetSKUNombreDescripcion()
        {
            // Se obtiene el nombre de usuario de la sesión
            string cliente = Session["usuario"].ToString();
            // Se obtienen los SKUs presentes en el inventario del usuario
            var inventarioSKUs = db.Inventario.Where(i => i.Cliente == cliente).Select(i => i.SKU).ToList();
            // Se obtienen los datos de los artículos que no están en el inventario del usuario
            var data = db.Zapatillas.Where(z => !inventarioSKUs.Contains(z.SKU)).Select(x => new { x.SKU, x.Nombre, x.Descripcion }).ToList();
            // Se devuelve la lista de datos en formato JSON
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Método para agregar zapatillas al inventario del usuario.
        /// </summary>
        /// <param name="zapas">Arreglo de zapatillas a agregar.</param>
        /// <returns>ActionResult que indica el éxito de la operación.</returns>
        public ActionResult AddZapas(ZapatillasAdd[] zapas)
        {
            // Se obtiene el nombre de usuario de la sesión
            string cliente = Session["usuario"].ToString();

            foreach (ZapatillasAdd z in zapas)
            {
                var existe = db.Inventario.Where(x => x.SKU == z.SKU).ToList();
                var existe2 = db.Inventario.Where(x => x.SKU == z.SKU).ToList();

                // Si el artículo ya existe en el inventario, se devuelve un mensaje de error
                if (existe.Any())
                    return Json(new { success = false, message = "El artículo con identificador '" + z.SKU + "' ya se encuentra en tu inventario." });

                // Si el artículo no existe, se devuelve un mensaje de error
                if (!existe2.Any())
                    return Json(new { success = false, message = "No se ha encontrado ningun artículo con identificador '" + z.SKU + "'." });

                // Se crea un nuevo artículo en el inventario y se guarda en la base de datos
                var data = new Inventario
                {
                    Cliente = cliente,
                    SKU = z.SKU,
                    Precio = z.Precio
                };

                db.Inventario.Add(data);
                db.SaveChanges();
            }

            // Se devuelve un mensaje de éxito
            return Json(new { success = true });
        }

        /// <summary>
        /// Método para generar un informe del inventario del usuario.
        /// </summary>
        /// <returns>ActionResult que indica el éxito de la operación.</returns>
        public ActionResult Informe()
        {
            // Se obtiene el nombre de usuario de la sesión
            string cliente = Session["usuario"].ToString();
            // Se obtienen los datos del inventario del usuario
            var data = db.Inventario.Where(x => x.Cliente == cliente).GroupBy(x => x.SKU).
                Join(db.Zapatillas, inv => inv.Key, zap => zap.SKU, (inv, zap) => new
                {
                    SKU = inv.Key,
                    zap.Nombre,
                    Cantidad = inv.Sum(x => x.Cantidad),
                    inv.FirstOrDefault().Precio
                })
            .ToList();

            // Se crea un DataTable para almacenar los datos del informe
            var DTInforme = new DataSetInformes().Tables["Inventario"];
            // Se agregan los datos al DataTable
            foreach (var linea in data)
                DTInforme.Rows.Add(linea.SKU, linea.Nombre, linea.Cantidad, linea.Precio);

            // Se crea un objeto DatosInforme para almacenar los datos del informe
            DatosInforme datosInforme = new DatosInforme();
            datosInforme.OrigenesDatos = new ReportDataSource[] { new ReportDataSource("Inventario", DTInforme) };
            datosInforme.DireccionInforme = "InformeInventario.rdlc";

            // Se guarda el objeto DatosInforme en la sesión
            Session["Informe_Datos"] = datosInforme;
            // Se devuelve un código de estado HTTP OK
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

    }
}