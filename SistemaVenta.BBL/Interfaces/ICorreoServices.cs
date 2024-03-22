using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BBL.Interfaces
{
    public interface ICorreoServices
    {
        Task<bool> EnviarCorreo(string CorreoDestino, string asunto, string mensaje);
    }
}
