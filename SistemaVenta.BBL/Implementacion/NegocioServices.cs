using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.Entity;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;

namespace SistemaVenta.BBL.Implementacion
{
    public class NegocioServices : INegocioServices
    {
        private readonly IGenericRepository<Negocio> _repository;
        private readonly IFireBaseServices _firebaseServices; // Para subir las imagines

        public NegocioServices(IGenericRepository<Negocio> repository, IFireBaseServices firebaseServices)
        {
            _firebaseServices = firebaseServices;
            _repository = repository;
        }
        public async Task<Negocio> Obtener()
        {
            try
            {
                Negocio negocioObtenido = await _repository.Obtener(n => n.IdNegocio == 1);

                return negocioObtenido;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Negocio> GuardarCambios(Negocio entidad, Stream Logo = null, string NombreLogo = "")
        {
            try
            {
                Negocio negocioObtenido = await _repository.Obtener(n => n.IdNegocio == 1);

                negocioObtenido.NumeroDocumento = entidad.NumeroDocumento;
                negocioObtenido.Nombre = entidad.Nombre;
                negocioObtenido.Correo = entidad.Correo;
                negocioObtenido.Direccion = entidad.Direccion;
                negocioObtenido.Telefono = entidad.Telefono;
                negocioObtenido.PorcentajeImpuesto = entidad.PorcentajeImpuesto;
                negocioObtenido.SimboloMoneda = entidad.SimboloMoneda;
                // En caso de ser vacio, quedarse con el nombre que llega por parametro (NombreLogo)
                negocioObtenido.NombreLogo = negocioObtenido.NombreLogo == "" ? NombreLogo : negocioObtenido.NombreLogo; 

                if(Logo != null)
                {
                    string urlLogo = await _firebaseServices.SubirStorage(Logo, "carpeta_logo", negocioObtenido.NombreLogo);
                    negocioObtenido.UrlLogo = urlLogo;
                }
                await _repository.Editar(negocioObtenido);

                return negocioObtenido;
            }
            catch 
            {
                throw;
            }
        }
    }
}
