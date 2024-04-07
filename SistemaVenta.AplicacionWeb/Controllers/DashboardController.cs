using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BBL.Interfaces;


namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashBoardServices _dashBoardServices;

        public DashboardController(IDashBoardServices dashBoardServices)
        {
            _dashBoardServices = dashBoardServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VmDashboard> gResponse = new GenericResponse<VmDashboard> ();
            try
            {
                VmDashboard vmDashboard = new VmDashboard();
                vmDashboard.TotalVentas = await _dashBoardServices.TotalVentasUltimaSemana();
                vmDashboard.TotalIngresos = await _dashBoardServices.TotalIngresosUltimaSemana();
                vmDashboard.TotalProductos = await _dashBoardServices.TotalProductos();
                vmDashboard.TotalCategorias = await _dashBoardServices.TotalCategorias();

                List<VmVentasSemana> listaVentaSemana = new List<VmVentasSemana>();
                List<VmProductosSemana> listaProductosSemana = new List<VmProductosSemana>();

                foreach (KeyValuePair<string, int> item in await _dashBoardServices.VentasUltimaSemana())
                {
                    listaVentaSemana.Add(new VmVentasSemana()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }
                foreach (KeyValuePair<string, int> item in await _dashBoardServices.ProductosTopUltimaSemana())
                {
                    listaProductosSemana.Add(new VmProductosSemana()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value
                    });
                }

                vmDashboard.VentasUltimaSemana = listaVentaSemana;
                vmDashboard.ProductosTopUltimaSemana = listaProductosSemana;

                gResponse.Estado = true;
                gResponse.Objeto = vmDashboard;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
