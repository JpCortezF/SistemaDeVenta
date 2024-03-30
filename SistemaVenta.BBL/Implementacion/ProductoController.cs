using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Implementacion
{
    public class ProductoController : IProductoServices
    {
        private readonly IGenericRepository<Producto> _repository;
        private readonly IFireBaseServices _fireBaseServices;

        public ProductoController(IGenericRepository<Producto> repository, IFireBaseServices fireBaseServices)
        {
            _repository = repository;
            _fireBaseServices = fireBaseServices;
        }
        public async Task<List<Producto>> Lista()
        {
            IQueryable<Producto> query = await _repository.Consultar();
            return query.Include(c => c.IdCategoriaNavigation).ToList(); // Se agrega la Categoria.
        }

        public async Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            Producto productoExiste = await _repository.Obtener(p => p.CodigoBarra == entidad.CodigoBarra);

            if (productoExiste != null)
            {
                throw new TaskCanceledException("El código de barra ya existe.");
            }

            try
            {
                entidad.NombreImagen = nombreImagen;
                if(imagen != null)
                {
                    string urlImagen = await _fireBaseServices.SubirStorage(imagen, "carpeta_producto", nombreImagen);
                    entidad.UrlImagen = urlImagen;
                }
                Producto productoCreado = await _repository.Crear(entidad); 

                if(productoCreado.IdProducto == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el producto");
                }
                IQueryable<Producto> query = await _repository.Consultar(p => p.IdProducto == productoCreado.IdProducto);
                productoCreado = query.Include(c => c.IdCategoriaNavigation).First(); // Se agrega la Categoria del producto.

                return productoCreado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Producto> Editar(Producto entidad, Stream imagen = null)
        {
            // Comprobamos que el codigo de barra no se repita en nuestra base de datos.
            Producto productoExiste = await _repository.Obtener(p => p.CodigoBarra == entidad.CodigoBarra && p.IdProducto != entidad.IdProducto);

            if(productoExiste != null)
            {
                throw new TaskCanceledException("El código ya se encuentra asignado a otro producto.");
            }
            try
            {
                IQueryable<Producto> queryProducto = await _repository.Consultar(p => p.IdProducto == entidad.IdProducto);
                Producto editarProducto = queryProducto.First();
                editarProducto.CodigoBarra = entidad.CodigoBarra;
                editarProducto.Marca = entidad.Marca;
                editarProducto.Descripcion = entidad.Descripcion;
                editarProducto.IdCategoria = entidad.IdCategoria;
                editarProducto.Stock = entidad.Stock;
                editarProducto.Precio = entidad.Precio;
                editarProducto.Stock = entidad.Stock;
                editarProducto.EsActivo = entidad.EsActivo;

                if(imagen != null)
                {
                    string urlImagen = await _fireBaseServices.SubirStorage(imagen, "carpeta_producto", editarProducto.NombreImagen);
                    editarProducto.UrlImagen = urlImagen;
                }
                bool respuesta = await _repository.Editar(editarProducto);
                if(!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el producto");
                }
                Producto productoEditado = queryProducto.Include(c => c.IdCategoriaNavigation).First();

                return productoEditado;
            }
            catch 
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idProducto)
        {
            try
            {
                Producto productoExiste = await _repository.Obtener(p => p.IdProducto == idProducto);

                if(productoExiste == null)
                {
                    throw new TaskCanceledException("El producto no existe");
                }

                string nombreImagen = productoExiste.NombreImagen;

                bool respuesta = await _repository.Eliminar(productoExiste);
                if (respuesta)
                {
                    await _fireBaseServices.EliminarStorage("carpeta_producto", nombreImagen);
                }
                return respuesta;
            }
            catch 
            {
                throw;
            }
        }
    }
}
