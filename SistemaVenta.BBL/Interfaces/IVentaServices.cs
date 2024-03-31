using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Interfaces
{
    public interface IVentaServices
    {
        Task<List<Producto>> ObtenerProductos(string busqueda);
        Task<Venta> Registrar(Venta entidad);
        Task<List<Venta>> HistorialVenta(string numeroDeVenta, string fechaInicio, string fechaFin);
        Task<Venta> DetalleVenta(string numeroVenta);
        Task<List<DetalleVenta>> ReporteVenta(string fechaInicio, string fechaFin);
    }
}
