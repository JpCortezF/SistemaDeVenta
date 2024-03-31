using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Implementacion
{
    public class VentaServices : IVentaServices
    {
        private readonly IGenericRepository<Producto> _productRepository;
        private readonly IVentaRepository _ventaRepository;

        public VentaServices(IGenericRepository<Producto> productRepository, IVentaRepository ventaRepository)
        {
            _productRepository = productRepository;
            _ventaRepository = ventaRepository;
        }

        public async Task<List<Producto>> ObtenerProductos(string busqueda)
        {
            IQueryable<Producto> query = await _productRepository.Consultar
            (p => p.EsActivo == true && p.Stock > 0 && string.Concat(p.CodigoBarra, p.Marca, p.Descripcion).Contains(busqueda));

            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            try
            {
                return await _ventaRepository.Registrar(entidad);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Venta>> HistorialVenta(string numeroDeVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepository.Consultar();
            fechaInicio = fechaInicio is null ? "" : fechaInicio;
            fechaFin = fechaFin is null ? "" : fechaFin;

            if(fechaInicio != "" && fechaFin != "")
            {
                DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-AR"));
                DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-AR"));

                return query.Where(v => // Filtro para buscar entre 2 fechas
                v.FechaRegistro.Value.Date >= fecha_inicio.Date &&
                v.FechaRegistro.Value.Date <= fecha_fin.Date
                )
                .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Include(u => u.IdUsuarioNavigation)
                .Include(dv => dv.DetalleVenta)
                .ToList();
            }
            else
            {
                return query.Where(v => v.NumeroVenta == numeroDeVenta)
                .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Include(u => u.IdUsuarioNavigation)
                .Include(dv => dv.DetalleVenta)
                .ToList();
            }
            
        }

        public async Task<Venta> DetalleVenta(string numeroVenta)
        {
            IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.NumeroVenta == numeroVenta);

            return query
            .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
            .Include(u => u.IdUsuarioNavigation)
            .Include(dv => dv.DetalleVenta)
            .First();
        }

        public async Task<List<DetalleVenta>> ReporteVenta(string fechaInicio, string fechaFin)
        {
            DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-AR"));
            DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-AR"));

            List<DetalleVenta> lista = await _ventaRepository.Reporte(fecha_inicio, fecha_fin);
            return lista;
        }
    }
}
