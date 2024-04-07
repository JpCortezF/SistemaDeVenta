using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.Entity;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using System.Globalization;

namespace SistemaVenta.BBL.Implementacion
{
    public class DashBoardServices : IDashBoardServices
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<DetalleVenta> _detalleVentaRepository;
        private readonly IGenericRepository<Categoria> _categoriaRepository;
        private readonly IGenericRepository<Producto> _productoRepository;
        private DateTime fechaInicio = DateTime.Now;

        public DashBoardServices(IVentaRepository ventaRepository, IGenericRepository<DetalleVenta> detalleVentaRepository, IGenericRepository<Categoria> categoriaRepository, IGenericRepository<Producto> productoRepository)
        {
            _ventaRepository = ventaRepository;
            _detalleVentaRepository = detalleVentaRepository;
            _categoriaRepository = categoriaRepository;
            _productoRepository = productoRepository;

            fechaInicio = fechaInicio.AddDays(-7);
        }
        public async Task<int> TotalVentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.FechaRegistro.Value.Date >= fechaInicio.Date);
                int total = query.Count();
                return total;
            }
            catch 
            {
                throw;
            }
        }

        public async Task<string> TotalIngresosUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.FechaRegistro.Value.Date >= fechaInicio.Date);

                decimal resultado = query.Select(v => v.Total).Sum(v => v.Value); // Seleccionamos una columna y sumamos sus valores

                return Convert.ToString(resultado, new CultureInfo("es-AR"));
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> TotalProductos()
        {
            try
            {
                IQueryable<Producto> query = await _productoRepository.Consultar(); // Obtenemos todos los productos
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> TotalCategorias()
        {
            try
            {
                IQueryable<Categoria> query = await _categoriaRepository.Consultar();
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.FechaRegistro.Value.Date >= fechaInicio.Date);

                Dictionary<string, int> resultado = query
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderByDescending(g => g.Key) // Ordenamos por fecha de forma descendiente
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })  // con Select creamos un objeto con fecha y total
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total); // a las propiedades keySelector y elementSelector se le asignan los valores

                return resultado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> ProductosTopUltimaSemana()
        {
            try
            {
                IQueryable<DetalleVenta> query = await _detalleVentaRepository.Consultar();

                Dictionary<string, int> resultado = query
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date)
                    .GroupBy(dv => dv.DescripcionProducto).OrderByDescending(g => g.Count()) // Se ordena del producto más vendido al menos vendido
                    .Select(dv => new { producto = dv.Key, total = dv.Count() })  // con Select creamos un objeto con fecha y total
                    .ToDictionary(keySelector: r => r.producto, elementSelector: r => r.total); // a las propiedades keySelector y elementSelector se le asignan los valores

                return resultado;
            }
            catch
            {
                throw;
            }
        }

    }
}
