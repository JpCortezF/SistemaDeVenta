using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using Newtonsoft.Json;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoServices _productoServices;
        private readonly IMapper _mapper;

        public ProductoController(IProductoServices productoServices, IMapper mapper, ICategoriaServices categoriaServices)
        {
            _productoServices = productoServices;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VmProducto> vmProductosLista = _mapper.Map<List<VmProducto>>(await _productoServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmProductosLista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile imagen, [FromForm] string modelo)
        {
            GenericResponse<VmProducto> gResponse = new GenericResponse<VmProducto>();
            try
            {
                VmProducto vmProducto = JsonConvert.DeserializeObject<VmProducto>(modelo);
                string nombreImagen = "";
                Stream imagenStream = null;

                if(imagen != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombreEnCodigo, extension);
                    imagenStream = imagen.OpenReadStream();
                }
                Producto productoCreado = await _productoServices.Crear(_mapper.Map<Producto>(vmProducto), imagenStream, nombreImagen);
                vmProducto = _mapper.Map<VmProducto>(productoCreado);

                gResponse.Estado = true;
                gResponse.Objeto = vmProducto;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string modelo)
        {
            GenericResponse<VmProducto> gResponse = new GenericResponse<VmProducto>();
            try
            {
                VmProducto vmProducto = JsonConvert.DeserializeObject<VmProducto>(modelo);
                Stream imagenStream = null;
                string nombreImagen = "";

                if (imagen != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombreEnCodigo, extension);
                    imagenStream = imagen.OpenReadStream();
                }
                Producto productoEditado = await _productoServices.Editar(_mapper.Map<Producto>(vmProducto), imagenStream, nombreImagen);
                vmProducto = _mapper.Map<VmProducto>(productoEditado);

                gResponse.Estado = true;
                gResponse.Objeto = vmProducto;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idProducto)
        {
            GenericResponse<VmCategoria> gResponse = new GenericResponse<VmCategoria>();
            try
            {
                gResponse.Estado = await _productoServices.Eliminar(idProducto);
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
