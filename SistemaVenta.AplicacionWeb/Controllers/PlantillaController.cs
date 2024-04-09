using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BBL.Interfaces;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioServices _negocioServices;
        private readonly IVentaServices _ventaServices;

        public PlantillaController(IMapper mapper, INegocioServices negocioServices, IVentaServices ventaServices)
        {
            _mapper = mapper;
            _negocioServices = negocioServices;
            _ventaServices = ventaServices;
        }

        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";

            return View();
        }

        public async Task<IActionResult> PDFVenta(string numeroVenta)
        {
            VmVenta vmVenta = _mapper.Map<VmVenta>(await _ventaServices.Detalle(numeroVenta));
            VmNegocio vmNegocio = _mapper.Map<VmNegocio>(await _negocioServices.Obtener());

            VmPDFVenta modelo = new VmPDFVenta();
            modelo.Negocio = vmNegocio;
            modelo.Venta = vmVenta;

            return View(modelo);
        }

        public IActionResult RestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;

            return View();
        }
    }
}
