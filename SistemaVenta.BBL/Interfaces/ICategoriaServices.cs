using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Interfaces
{
    public interface ICategoriaServices
    {
        Task<List<Categoria>> Lista();
        Task<Categoria> Crear(Categoria entidad);
        Task<Categoria> Editar(Categoria entidad);
        Task<bool> Eliminar(int idCategoria);
    }
}
