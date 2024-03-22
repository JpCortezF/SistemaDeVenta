using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BBL.Interfaces
{
    public interface IUtilidadesServices
    {
        string GenerarClave();

        string ConvertirSha256(string input);
    }
}
