using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Interfaces
{
    public interface IProductoServices
    {
        Task<List<Producto>> Lista();
        Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen = "");
        Task<Producto> Editar(Producto entidad, Stream imagen = null, string nombreImagen = "");
        Task<bool> Eliminar(int idProducto);
    }
}
