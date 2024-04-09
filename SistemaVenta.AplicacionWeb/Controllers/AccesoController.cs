using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioServices _usuarioServices;

        public AccesoController(IUsuarioServices usuarioServices)
        {
            _usuarioServices = usuarioServices;
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult RestablecerClave()
        {


            return View();
        }

       [HttpPost]
        public async Task<IActionResult> Login(VmUsuarioLogin modelo)
        {
            Usuario usuarioEncontrado = await _usuarioServices.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);

            if(usuarioEncontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();  
            }
            ViewData["Mensaje"] = null;

            List<Claim> claims = new List<Claim>() // Guardamos la información del usuario una vez logeado.
            {
                new Claim(ClaimTypes.Name, usuarioEncontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuarioEncontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuarioEncontrado.IdRol.ToString()),
                new Claim("UrlFoto", usuarioEncontrado.UrlFoto),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = modelo.MantenerSesion
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VmUsuarioLogin modelo)
        {
            try
            {
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";

                bool resultado = await _usuarioServices.RestablecerClave(modelo.Correo, urlPlantillaCorreo);

                if(resultado)
                {
                    ViewData["Mensaje"] = "Su contraseña fue restablecida. Revise su correo";
                    ViewData["MensajeError"] = null;
                }
                else
                {
                    ViewData["Mensaje"] = null;
                    ViewData["MensajeError"] = "Ocurrio un problema. Por favor intentelo más tarde";
                }
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = null;
                ViewData["MensajeError"] = ex.Message;
            }
            return View();
        }
    }
}
