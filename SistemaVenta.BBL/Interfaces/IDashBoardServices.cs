using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BBL.Interfaces
{
    public interface IDashBoardServices
    {
        Task<int> TotalVentasUltimaSemana();
        Task<string> TotalIngresosUltimaSemana();
        Task<int> TotalProductos();
        Task<int> TotalCategorias();
        Task<Dictionary<string, int>> VentasUltimaSemana();
        Task<Dictionary<string, int>> ProductosTopUltimaSemana();

    }
}
