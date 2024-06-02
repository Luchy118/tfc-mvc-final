using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        // Conexión a la base de datos.
        string conexion = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=TFC;Integrated Security=True";
        static TFCEntities db = new TFCEntities();

        /// <summary>
        /// Acción para la vista principal del Home.
        /// </summary>
        /// <returns>Vista principal del Home.</returns>
        public ActionResult Index()
        {
            // Inicialmente, se asume que el usuario no tiene permisos de administrador.
            ViewBag.PermisoAdmin = false;

            // Si hay un usuario logueado, se comprueba si es administrador.
            if (Session["usuario"] != null)
            {
                string user = Session["usuario"].ToString();
                Usuarios usuario = db.Usuarios.Where(x => x.Usuario == user).First();

                // Si el usuario es administrador, se actualiza la ViewBag correspondiente.
                if (usuario.Administrador == 1)
                    ViewBag.PermisoAdmin = true;
            }

            // Se obtiene la fecha de hace un mes.
            DateTime haceUnMes = DateTime.Now.AddMonths(-1);

            // Se obtienen las zapatillas lanzadas en el último mes, ordenadas por fecha de lanzamiento.
            var lanzamientosUltimoMes = db.Zapatillas.Where(z => z.FechaLanzamiento >= haceUnMes).OrderByDescending(x => x.FechaLanzamiento).ToList();
            ViewBag.lanzamientosUltimoMes = lanzamientosUltimoMes;
            // Se obtiene una lista de marcas distintas de las zapatillas lanzadas en el último mes.
            ViewBag.filtroMarcas = lanzamientosUltimoMes.Select(x => x.Marca).Distinct().ToList();

            return View();
        }

        /// <summary>
        /// Acción para la vista de login.
        /// </summary>
        /// <returns>Vista de login.</returns>
        public ActionResult Login()
        {
            // Se configura la ViewBag para no renderizar ciertos elementos en la vista de login.
            ViewBag.renderBelow = "false";

            return View();
        }

        /// <summary>
        /// Acción para cerrar sesión.
        /// </summary>
        /// <returns>JSON indicando éxito.</returns>
        public ActionResult Logout()
        {
            // Se elimina la sesión del usuario.
            Session["usuario"] = null;

            return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Método para validar el login de un usuario.
        /// </summary>
        /// <param name="u">Datos del usuario.</param>
        /// <returns>Redirección o JSON indicando el resultado.</returns>
        public ActionResult ValidaLogin(Usuarios u)
        {
            // Se usa una conexión SQL para comprobar las credenciales del usuario.
            using (SqlConnection con = new SqlConnection(conexion))
            {
                DataTable dt = new DataTable();
                con.Open();

                // Consulta para obtener el usuario por email.
                string queryString = "SELECT * FROM Usuarios WHERE Email = @email";
                SqlCommand command = new SqlCommand(queryString, con);
                command.Parameters.AddWithValue("@email", u.Email);

                // Se ejecuta la consulta y se cargan los resultados en un DataTable.
                SqlDataReader reader = command.ExecuteReader();
                dt.Load(reader);
                con.Close();

                // Si no se encuentra ningún usuario con ese email, se devuelve un mensaje de error.
                if (dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "No existe ninguna cuenta de usuario asociada a ese correo electrónico." });
                }
                else
                {
                    // Si se encuentra el usuario, se verifica la contraseña.
                    if (VerifyPassword(u.Contrasenia, dt.Rows[0]["Contrasenia"].ToString()))
                    {
                        string user = dt.Rows[0]["Usuario"].ToString();
                        Session["usuario"] = user;

                        Usuarios usuario = db.Usuarios.Where(x => x.Usuario == user).First();

                        // Si el usuario es administrador, se redirige al área de administración.
                        if (usuario.Administrador == 1)
                        {
                            ViewBag.redirAdm = true;
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                            // Si no es administrador, se redirige al área de usuarios.
                            return RedirectToAction("Index", "Usuarios");
                    }
                    else
                    {
                        // Si la contraseña es incorrecta, se devuelve un mensaje de error.
                        return Json(new { success = false, message = "La contraseña es errónea." });
                    }
                }
            }
        }

        /// <summary>
        /// Método para validar el registro de un nuevo usuario.
        /// </summary>
        /// <param name="u">Datos del usuario.</param>
        /// <returns>JSON indicando el resultado.</returns>
        public ActionResult ValidaRegistro(Usuarios u)
        {
            // Se usa una conexión SQL para comprobar si ya existe un usuario con el mismo email o nombre de usuario.
            using (SqlConnection con = new SqlConnection(conexion))
            {
                DataTable dt = new DataTable();
                con.Open();

                // Consulta para obtener usuarios por email o nombre de usuario.
                string queryString = "SELECT * FROM Usuarios WHERE Email = @email OR Usuario = @usuario";
                SqlCommand command = new SqlCommand(queryString, con);
                command.Parameters.AddWithValue("@email", u.Email);
                command.Parameters.AddWithValue("@usuario", u.Usuario);

                // Se ejecuta la consulta y se cargan los resultados en un DataTable.
                SqlDataReader reader = command.ExecuteReader();
                dt.Load(reader);
                con.Close();

                // Si no se encuentran usuarios con el mismo email o nombre de usuario, se procede a registrar el nuevo usuario.
                if (dt.Rows.Count == 0)
                {
                    // Se hace hash de la contraseña antes de guardar en la base de datos.
                    u.Contrasenia = HashPassword(u.Contrasenia);
                    u.FechaRegistro = DateTime.Now;
                    db.Usuarios.Add(u);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    // Si ya existe un usuario con el mismo email o nombre de usuario, se devuelve un mensaje de error.
                    return Json(new { success = false, message = "El nombre de usuario y/o el correo electrónico ya está(n) en uso." });
                }
            }
        }

        /// <summary>
        /// Método para hacer hash de una contraseña.
        /// </summary>
        /// <param name="password">Contraseña en texto plano.</param>
        /// <returns>Hash de la contraseña.</returns>
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Método para verificar una contraseña.
        /// </summary>
        /// <param name="inputPassword">Contraseña ingresada por el usuario.</param>
        /// <param name="storedHash">Hash de la contraseña almacenada.</param>
        /// <returns>True si la contraseña es correcta, false en caso contrario.</returns>
        public bool VerifyPassword(string inputPassword, string storedHash)
        {
            string inputHash = HashPassword(inputPassword);
            return inputHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Método para generar un PDF.
        /// </summary>
        /// <param name="id">Identificador del documento.</param>
        /// <returns>Archivo PDF.</returns>
        public ActionResult GeneratePdf(string id)
        {
            var PDFResult = new ActionAsPdf(id)
            {
                FileName = id + ".PDF"
            };

            return PDFResult;
        }
    }
}
