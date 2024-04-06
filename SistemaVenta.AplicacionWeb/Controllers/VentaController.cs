using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;

using DinkToPdf;
using DinkToPdf.Contracts;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class VentaController : Controller
    {
        private readonly ITipoDocumentoVentaServices _tipoDocumentoServicio;
        private readonly IVentaServices _ventaServices;
        private readonly IMapper _mapper;
        private readonly IConverter _converter;

        public VentaController(ITipoDocumentoVentaServices tipoDocumentoServicio, IVentaServices ventaServices, IMapper mapper, IConverter converter)
        {
            _tipoDocumentoServicio = tipoDocumentoServicio;
            _ventaServices = ventaServices;
            _mapper = mapper;
            _converter = converter;

        }
        public IActionResult NuevaVenta()
        {
            return View();
        }

        public IActionResult HistorialVenta()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoDocumentoVenta()
        {
            List<VmTipoDocumentoVenta> vmListaDocumentos = _mapper.Map<List<VmTipoDocumentoVenta>>(await _tipoDocumentoServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaDocumentos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string busqueda)
        {
            List<VmProducto> vmListaProductos = _mapper.Map<List<VmProducto>>(await _ventaServices.ObtenerProductos(busqueda));
            return StatusCode(StatusCodes.Status200OK, vmListaProductos);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VmVenta modelo)
        {
            GenericResponse<VmVenta> gResponse = new GenericResponse<VmVenta>();

            try
            {
                modelo.IdUsuario = 4;

                Venta venta = await _ventaServices.Registrar(_mapper.Map<Venta>(modelo)); // De VmVenta a Venta
                modelo = _mapper.Map<VmVenta>(venta); // De Venta a VmVenta

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = "Ocurrió un error al registrar la venta: " + ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            List<VmVenta> vmHistorialVenta = _mapper.Map<List<VmVenta>>(await _ventaServices.Historial(numeroVenta, fechaInicio, fechaFin));
            return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);
        }

        public IActionResult MostrarPDFVenta(string numeroVenta)
        {
            string urlPlantillaVista = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFVenta?numeroVenta={numeroVenta}";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        Page = urlPlantillaVista
                    }
                }
            };

            var archivoPDF = _converter.Convert(pdf);

            return File(archivoPDF, "application/pdf");
        }
    }
}
