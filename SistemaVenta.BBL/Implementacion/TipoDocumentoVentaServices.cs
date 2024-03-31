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
    public class TipoDocumentoVentaServices : ITipoDocumentoVentaServices
    {
        private readonly IGenericRepository<TipoDocumentoVenta> _repository;

        public TipoDocumentoVentaServices(IGenericRepository<TipoDocumentoVenta> repository)
        {
            _repository = repository;
        }

        public async Task<List<TipoDocumentoVenta>> Lista()
        {
            IQueryable<TipoDocumentoVenta> query = await _repository.Consultar();
            return query.ToList();
        }
    }
}
