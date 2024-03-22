using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.BBL.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;

namespace SistemaVenta.BBL.Implementacion
{
    public class FireBaseServices : IFireBaseServices
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        public FireBaseServices(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> SubirStorage(Stream streamArchivo, string carpetaDestino, string nombreArchivo)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                // Guardo los valores de la columna 'Propiedad' y 'Valor' en el Dictionary
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var authentication = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await authentication.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                    ).Child(Config[carpetaDestino])
                    .Child(nombreArchivo)
                    .PutAsync(streamArchivo, cancellation.Token);

                UrlImagen = await task;

            }
            catch
            {
                UrlImagen = "";
            }
            return UrlImagen;
        }

        public async Task<bool> EliminarStorage(string carpetaDestino, string nombreArchivo)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                // Guardo los valores de la columna 'Propiedad' y 'Valor' en el Dictionary
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var authentication = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await authentication.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                    ).Child(Config[carpetaDestino])
                    .Child(nombreArchivo)
                    .DeleteAsync();

                 await task;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
