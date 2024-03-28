using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;


namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioServices _usuarioServices;
        private readonly IRolServices _rolServices;
        private readonly IMapper _mapper;

        public UsuarioController(IUsuarioServices usuarioServices, IRolServices rolServices, IMapper mapper)
        {
            _usuarioServices = usuarioServices;
            _rolServices = rolServices;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            var lista = await _rolServices.Lista(); // .Lista() retorna List<Rol>
            List<VmRol> vmListaRoles = _mapper.Map<List<VmRol>>(lista);  // Mappeo para castearlo a List<VmRol>

            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _usuarioServices.Lista(); 
            List<VmUsuario> vmListaUsuario = _mapper.Map<List<VmUsuario>>(lista); 

            return StatusCode(StatusCodes.Status200OK, new { data = vmListaUsuario }); // Nuevo formato para datatable de JQuery {data = info}
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VmUsuario> gResponse = new GenericResponse<VmUsuario>();
            try
            {
                VmUsuario vmUsuario = JsonConvert.DeserializeObject<VmUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreEnCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuarioCreado = await _usuarioServices.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);

                vmUsuario = _mapper.Map<VmUsuario>(usuarioCreado);

                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VmUsuario> gResponse = new GenericResponse<VmUsuario>();
            try
            {
                VmUsuario vmUsuario = JsonConvert.DeserializeObject<VmUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreEnCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuarioEditado = await _usuarioServices.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);
                vmUsuario = _mapper.Map<VmUsuario>(usuarioEditado);

                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _usuarioServices.Eliminar(idUsuario);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                throw;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
