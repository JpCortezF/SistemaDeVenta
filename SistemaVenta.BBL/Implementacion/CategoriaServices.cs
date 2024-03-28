using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;


namespace SistemaVenta.BBL.Implementacion
{
    public class CategoriaServices : ICategoriaServices
    {
        private readonly IGenericRepository<Categoria> _repository;

        public CategoriaServices(IGenericRepository<Categoria> repository)
        {
            _repository = repository;
        }
        public async Task<List<Categoria>> Lista()
        {
            IQueryable<Categoria> query = await _repository.Consultar();
            return query.ToList();
        }

        public async Task<Categoria> Crear(Categoria entidad)
        {
            try
            {
                Categoria nuevaCategoria = await _repository.Crear(entidad);

                if(nuevaCategoria.IdCategoria == 0)
                {
                    throw new TaskCanceledException("No se pudo crear la categoria.");
                }
                return nuevaCategoria;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Categoria> Editar(Categoria entidad)
        {
            try
            {
                Categoria categoriaEncontrada = await _repository.Obtener(c => c.IdCategoria == entidad.IdCategoria);
                categoriaEncontrada.Descripcion = entidad.Descripcion;
                categoriaEncontrada.EsActivo = entidad.EsActivo;

                bool respuesta = await _repository.Editar(categoriaEncontrada);
                if(!respuesta)
                {
                    throw new TaskCanceledException("No se pudo modificar la categoria.");
                }

                return categoriaEncontrada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idCategoria)
        {
            try
            {
                Categoria categoriaEncontrada = await _repository.Obtener(c => c.IdCategoria == idCategoria);
                if(categoriaEncontrada == null)
                {
                    throw new TaskCanceledException("La categoria no existe.");
                }
                bool respuesta = await _repository.Eliminar(categoriaEncontrada);

                return respuesta;
            }
            catch
            {
                throw;
            }
        }
    }
}
