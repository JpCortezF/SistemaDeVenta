using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.BBL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;

namespace SistemaVenta.BBL.Implementacion
{
    public class UsuarioServices : IUsuarioServices
    {
        private readonly IGenericRepository<Usuario> _repository;
        private readonly IFireBaseServices _fireBaseServices;
        private readonly IUtilidadesServices _utilidadesServices;
        private readonly ICorreoServices _correoServices;

        public UsuarioServices(IGenericRepository<Usuario> repository, IFireBaseServices fireBaseServices, IUtilidadesServices utilidadesServices, ICorreoServices correoServices)
        {
            _repository = repository;
            _fireBaseServices = fireBaseServices;
            _utilidadesServices = utilidadesServices;
            _correoServices = correoServices;

        }
        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repository.Consultar();

            return query.Include(r => r.IdRolNavigation).ToList();
        }

        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            Usuario usuarios = await _repository.Obtener(u => u.Correo == entidad.Correo);
            
            if (usuarios == null)
            {
                try
                {
                    string generarClave = _utilidadesServices.GenerarClave();
                    entidad.Clave = _utilidadesServices.ConvertirSha256(generarClave);
                    entidad.NombreFoto = NombreFoto;

                    if(Foto != null)
                    {
                        string urlFoto = await _fireBaseServices.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                        entidad.UrlFoto = urlFoto;
                    }

                    Usuario nuevoUsuario = await _repository.Crear(entidad);
                    if(nuevoUsuario.IdUsuario == 0)
                    {
                        throw new TaskCanceledException("No se pudo crear el usuario");
                    }

                    if(UrlPlantillaCorreo != "")
                    {
                        UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", nuevoUsuario.Correo).Replace("[clave]", generarClave);

                        string htmlCorreo = "";

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        if(response.StatusCode == HttpStatusCode.OK)
                        {
                            using (Stream dataStream = response.GetResponseStream())
                            {
                                StreamReader readerStream = null;

                                if(response.CharacterSet == null)
                                {
                                    readerStream = new StreamReader(dataStream);
                                }
                                else
                                {
                                    readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                                    htmlCorreo = readerStream.ReadToEnd();
                                    response.Close();
                                    readerStream.Close();
                                }
                            }
                        }

                        if(htmlCorreo != "")
                        {
                            await _correoServices.EnviarCorreo(nuevoUsuario.Correo, "Cuenta creada", htmlCorreo);
                        }
                    }

                    IQueryable<Usuario> query = await _repository.Consultar(u => u.IdUsuario == nuevoUsuario.IdUsuario);
                    nuevoUsuario = query.Include(r => r.IdRolNavigation).First();

                    return nuevoUsuario;
                }
                catch(Exception ex)
                {
                    throw;
                }
            }
            else
            {
                throw new TaskCanceledException("El correo ya existe");
            }
        }

        public async Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            Usuario usuarios = await _repository.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);

            if (usuarios == null)
            {
                try
                {
                    IQueryable<Usuario> queryUsuario = await _repository.Consultar(u => u.IdUsuario == entidad.IdUsuario);

                    Usuario editarUsuario = queryUsuario.First();
                    editarUsuario.NombreFoto = entidad.NombreFoto;
                    editarUsuario.Correo = entidad.Correo;
                    editarUsuario.Telefono = entidad.Telefono;
                    editarUsuario.IdRol = entidad.IdRol;

                    if (editarUsuario.NombreFoto == "")
                    {
                        editarUsuario.NombreFoto = NombreFoto;
                    }
                    if (Foto != null)
                    {
                        string urlFoto = await _fireBaseServices.SubirStorage(Foto, "carpeta_usuario", editarUsuario.NombreFoto);
                        editarUsuario.UrlFoto = urlFoto;
                    }
                    bool respuesta = await _repository.Editar(editarUsuario);

                    if(!respuesta)
                    {
                        throw new TaskCanceledException("No se pudo editar el usuario");
                    }

                    Usuario usuarioEditado = queryUsuario.Include(r => r.IdRolNavigation).First();

                    return usuarioEditado;
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                throw new TaskCanceledException("El correo ya existe");
            }
        }
        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                Usuario usuarioEncontrado = await _repository.Obtener(u => u.IdUsuario == idUsuario);

                if(usuarioEncontrado == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }

                string nombreFoto = usuarioEncontrado.NombreFoto;
                bool respuesta = await _repository.Eliminar(usuarioEncontrado);

                if(respuesta)
                {
                    await _fireBaseServices.EliminarStorage("carpeta_usuario", nombreFoto);
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string claveEncriptada = _utilidadesServices.ConvertirSha256(clave);
            Usuario usuarioEncontrado = await _repository.Obtener(u => u.Correo.Equals(correo) && u.Clave.Equals(claveEncriptada));

            return usuarioEncontrado;
        }

        public async Task<Usuario> ObtenerPorId(int idUsuario)
        {
            IQueryable<Usuario> query = await _repository.Consultar(u => u.IdUsuario == idUsuario);

            Usuario resultado = query.Include(r => r.IdRolNavigation).FirstOrDefault();

            return resultado;
        }
        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuarioEncontrado = await _repository.Obtener(u => u.IdUsuario == entidad.IdUsuario);

                if(usuarioEncontrado == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }

                usuarioEncontrado.Correo = entidad.Correo;
                usuarioEncontrado.Telefono = entidad.Telefono;

                bool respuesta = await _repository.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> CambiarClave(int idUsuario, string claveActual, string nuevaClave)
        {
            try
            {
                Usuario usuarioEncontrado = await _repository.Obtener(u => u.IdUsuario == idUsuario);

                if (usuarioEncontrado == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }

                if(usuarioEncontrado.Clave != _utilidadesServices.ConvertirSha256(claveActual))
                {
                    throw new TaskCanceledException("La contraseña actual, no es correcta");
                }

                usuarioEncontrado.Clave = _utilidadesServices.ConvertirSha256(nuevaClave);

                bool respuesta = await _repository.Editar(usuarioEncontrado);

                return true;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo)
        {
            try
            {
                Usuario usuarioEncontrado = await _repository.Obtener(u => u.Correo == correo);

                if(usuarioEncontrado == null)
                {
                    throw new TaskCanceledException("No se encontró ningún usuario asociado al correo");
                }

                string generarClave = _utilidadesServices.GenerarClave();
                usuarioEncontrado.Clave = _utilidadesServices.ConvertirSha256(generarClave);

                UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", generarClave);

                string htmlCorreo = "";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader readerStream = null;

                        if (response.CharacterSet == null)
                        {
                            readerStream = new StreamReader(dataStream);
                        }
                        else
                        {
                            readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }
                    }
                }

                bool correoEnviado = false;

                if (htmlCorreo != "")
                {
                    correoEnviado = await _correoServices.EnviarCorreo(correo, "Contraseña restablecida", htmlCorreo);
                }

                if(!correoEnviado)
                {
                    throw new TaskCanceledException("Ocurrio un problema. Por favor inténtalo de nuevo más tarde");
                }

                bool respuesta = await _repository.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch
            {
                throw;
            }
        }
    }
}
