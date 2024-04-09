using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BBL.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IVentaServices _ventaServices;

        public ReporteController(IMapper mapper, IVentaServices ventaServices)
        {
            _mapper = mapper;
            _ventaServices = ventaServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReporteVenta(string fechaInicio, string fechaFin)
        {
            List<VmReporteVenta> vmListaReporte = _mapper.Map<List<VmReporteVenta>>(await _ventaServices.Reporte(fechaInicio, fechaFin));

            return StatusCode(StatusCodes.Status200OK, new { data = vmListaReporte });
        }
    }
}
