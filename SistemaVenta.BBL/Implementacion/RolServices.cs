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
    public class RolServices : IRolServices
    {
        private readonly IGenericRepository<Rol> _repository;

        public RolServices(IGenericRepository<Rol> repository)
        {
            _repository = repository;
        }
        public async Task<List<Rol>> Lista()
        {
            IQueryable<Rol> query = await _repository.Consultar();

            return query.ToList();
        }
    }
}
