using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;
using System.Diagnostics;

using System.Security.Claims;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUsuarioServices _usuarioServices;
        private readonly IMapper _mapper;


        public HomeController(IUsuarioServices usuarioServices, IMapper mapper)
        {
            _usuarioServices = usuarioServices;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VmUsuario> gResponse = new GenericResponse<VmUsuario>();
            try
            {
                ClaimsPrincipal claimUsuario = HttpContext.User;

                string idUsuario = claimUsuario.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier) // almacena el id del usuario
                .Select(c => c.Value).SingleOrDefault();

                VmUsuario usuario = _mapper.Map<VmUsuario>(await _usuarioServices.ObtenerPorId(int.Parse(idUsuario)));

                gResponse.Objeto = usuario;
                gResponse.Estado = true;
            }
            catch (Exception ex)
            {
                gResponse.Mensaje = ex.Message;
                gResponse.Estado = false;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody] VmUsuario modelo)
        {
            GenericResponse<VmUsuario> gResponse = new GenericResponse<VmUsuario>();
            try
            {
                ClaimsPrincipal claimUsuario = HttpContext.User;

                string idUsuario = claimUsuario.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier) // almacena el id del usuario
                .Select(c => c.Value).SingleOrDefault();

                Usuario entidad = _mapper.Map<Usuario>(modelo);

                entidad.IdUsuario = int.Parse(idUsuario);

                bool resultado = await _usuarioServices.GuardarPerfil(entidad);

                gResponse.Estado = resultado;
            }
            catch (Exception ex)
            {
                gResponse.Mensaje = ex.Message;
                gResponse.Estado = false;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] VmCambiarClave modelo)
        {
            GenericResponse<bool> gResponse = new GenericResponse<bool>();
            try
            {
                ClaimsPrincipal claimUsuario = HttpContext.User;

                string idUsuario = claimUsuario.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier) // almacena el id del usuario
                .Select(c => c.Value).SingleOrDefault();

                bool resultado = await _usuarioServices.CambiarClave(int.Parse(idUsuario), modelo.ClaveActual, modelo.ClaveNueva);

                gResponse.Estado = resultado;
            }
            catch (Exception ex)
            {
                gResponse.Mensaje = ex.Message;
                gResponse.Estado = false;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Acceso");
        }
    }
}