using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.DAL.Implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DBVENTAContext _dbContext;

        public VentaRepository(DBVENTAContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Venta> Registrar(Venta entidad)
        {           
            Venta ventaGenerada = new Venta();

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta dv in entidad.DetalleVenta)
                    {
                        // Consultar el producto asociado al detalle de venta
                        Producto productoEncontrado = _dbContext.Productos.Where(p => p.IdProducto == dv.IdProducto).First(); // Consulta de where. expresión Linq. Devuelve la primera coincidencia
                        productoEncontrado.Stock = productoEncontrado.Stock - dv.Cantidad; // disminuyo la cantidad de Stock
                        _dbContext.Productos.Update(productoEncontrado); // Actualizar el stock del producto
                    }
                    await _dbContext.SaveChangesAsync();

                    // Actualizar el número correlativo de ventas
                    NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos.Where(n => n.Gestion == "Venta").First();

                    correlativo.UltimoNumero++;
                    correlativo.FechaActualizacion = DateTime.Now;

                    _dbContext.NumeroCorrelativos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    // Generar número de venta
                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value)); // ceros = va a contener 6 veces cero
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value); // numeroVenta.Length = 7

                    entidad.NumeroVenta = numeroVenta;

                    await _dbContext.Venta.AddAsync(entidad);
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = entidad;

                    transaction.Commit(); // Una vez que pasa todo el proceso, llega al Commit. Y confirma la transacción.
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Deshacer la transacción en caso de error
                    throw;
                }

                return ventaGenerada;
            }
        }
        public async Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            // include sirve como Join entre las tablas
            List<DetalleVenta> listaResumen = await _dbContext.DetalleVenta
                .Include(v => v.IdVentaNavigation) // Include para ir a la tabla VENTA
                .ThenInclude(u => u.IdUsuarioNavigation) // ThenInclude porque ya me encuentro en la tabla VENTA
                .Include(v => v.IdVentaNavigation) // Include para volver a la tabla DETALLE
                .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date &&
                dv.IdVentaNavigation.FechaRegistro.Value.Date <= FechaFin.Date).ToListAsync();

            return listaResumen;
        }
    }
}
