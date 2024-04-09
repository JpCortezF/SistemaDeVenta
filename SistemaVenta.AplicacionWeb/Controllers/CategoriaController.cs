using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICategoriaServices _categoriaServices;

        public CategoriaController(IMapper mapper, ICategoriaServices categoriaServices)
        {
            _mapper = mapper;
            _categoriaServices = categoriaServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VmCategoria> vmCategoriaLista = _mapper.Map<List<VmCategoria>>(await _categoriaServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmCategoriaLista }); // Por medio de un objeto se le pasa la información a DataTable
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VmCategoria modelo)
        {
            GenericResponse<VmCategoria> gResponse = new GenericResponse<VmCategoria>();
            try
            {
                Categoria categoriaCreada = await _categoriaServices.Crear(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<VmCategoria>(categoriaCreada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VmCategoria modelo)
        {
            GenericResponse<VmCategoria> gResponse = new GenericResponse<VmCategoria>();
            try
            {
                Categoria categoriaEditada = await _categoriaServices.Editar(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<VmCategoria>(categoriaEditada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idCategoria)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _categoriaServices.Eliminar(idCategoria);

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
