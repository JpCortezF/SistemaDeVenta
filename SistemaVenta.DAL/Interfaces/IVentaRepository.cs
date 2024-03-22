using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.Entity;

namespace SistemaVenta.DAL.Interfaces
{
    public interface IVentaRepository : IGenericRepository<Venta>
    {
        Task<Venta> Registrar(Venta entidad); // Método para registrar una venta
        Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin); // Método para generar un reporte de ventas en un rango de fechas
    }
}
