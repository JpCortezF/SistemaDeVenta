﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Interfaces
{
    public interface ITipoDocumentoVentaServices
    {
        Task<List<TipoDocumentoVenta>> Lista();
    }
}