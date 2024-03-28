using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using System.Runtime.Intrinsics.X86;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class NegocioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioServices _negocioServices;

        public NegocioController(INegocioServices negocioServices, IMapper mapper)
        {
            _negocioServices = negocioServices;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            GenericResponse<VmNegocio> gResponse = new GenericResponse<VmNegocio>();

            try
            {
                VmNegocio vmNegocio = _mapper.Map<VmNegocio>(await _negocioServices.Obtener()); // Se convierte de Negocio a VmNegocio


                gResponse.Estado = true;
                gResponse.Objeto = vmNegocio;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios([FromForm] IFormFile logo, [FromForm] string modelo)
        {
            GenericResponse<VmNegocio> gResponse = new GenericResponse<VmNegocio>();

            try
            {
                VmNegocio vmNegocio = JsonConvert.DeserializeObject<VmNegocio>(modelo);

                string nombreLogo = "";
                Stream logoStream = null;

                if(logo != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N"); // Numeros y letras
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombreEnCodigo, extension);
                    logoStream = logo.OpenReadStream();
                }

                Negocio negocioGuardado = await _negocioServices.GuardarCambios(_mapper.Map<Negocio>(vmNegocio), logoStream, nombreLogo);

                vmNegocio = _mapper.Map<VmNegocio>(negocioGuardado);

                gResponse.Estado = true;
                gResponse.Objeto = vmNegocio;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
