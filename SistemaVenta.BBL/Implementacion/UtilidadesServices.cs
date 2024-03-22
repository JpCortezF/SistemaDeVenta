using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.BBL.Interfaces;
using System.Security.Cryptography;

namespace SistemaVenta.BBL.Implementacion
{
    public class UtilidadesServices : IUtilidadesServices
    {

        public string GenerarClave()
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6); // Formato N = generar numeros y letras (6 digitos)

            return clave;
        }

        public string ConvertirSha256(string input)
        {
            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;

                byte[] result = hash.ComputeHash(enc.GetBytes(input));

                foreach(byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

    }
}
