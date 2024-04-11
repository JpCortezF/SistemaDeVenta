$(document).ready(function () {

    $(".container-fluid").LoadingOverlay("show");

    fetch("/Home/ObtenerUsuario")
        .then(response => {
            $(".container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            if (responseJson.estado) {
                const objeto = responseJson.objeto

                $("#imgFoto").attr("src", objeto.urlFoto)

                $("#txtNombre").val(objeto.nombre)
                $("#txtCorreo").val(objeto.correo)
                $("#txTelefono").val(objeto.telefono)
                $("#txtRol").val(objeto.nombreRol)
            } else {
                swal("Lo sentimos", responseJson.mensaje, "error")
            }
        })
})

$("#btnGuardarCambios").click(function () {
    if ($("#txtCorreo").val().trim() == "") {
        toastr.warning("Debe completar el campo: Correo")
        $("#txtCorreo").focus()
        return;
    }
    if ($("#txTelefono").val().trim() == "") {
        toastr.warning("Debe completar el campo: Telefono")
        $("#txTelefono").focus()
        return;
    }

    swal({
        title: "¿Desea guardar los cambios?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Guardar",
        cancelButtonText: "Cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                let modelo = {
                    correo: $("#txtCorreo").val().trim(),
                    telefono: $("#txTelefono").val().trim()
                }

                fetch("/Home/GuardarPerfil", {
                    method: "POST",
                    headers: { "Content-Type": "application/json;" }, //charset=utf8"
                    body: JSON.stringify(modelo)
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) { // propiedad de gResponse en el Controller 
                            
                            swal("Listo!", "Cambios guardados exitosamente", "success")
                        }
                        else {
                            swal("Lo sentimos", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    )
})


$("#btnCambiarClave").click(function () {

    const inputs = $("input.input-validar").serializeArray();

    const inputsSinValor = inputs.filter((item) => item.value.trim() == ""); // devuelve el input sin valor

    if (inputsSinValor.length > 0) {
        const mensaje = `Debe completar el campo: "${inputsSinValor[0].name}"`
        toastr.warning("", mensaje)
        $(`input[name]="${inputsSinValor[0].name}"`).focus()
        return;
    }

    if ($("#txtClaveNueva").val().trim() != $("#txtConfirmarClave").val().trim()) {
        toastr.warning("Las contraseñas no coinciden");
        return;
    }

    let modelo = {
        claveActual: $("#txtClaveActual").val().trim(),
        claveNueva: $("#txtClaveNueva").val().trim()
    }

    fetch("/Home/CambiarClave", {
        method: "POST",
        headers: { "Content-Type": "application/json;" }, //charset=utf8"
        body: JSON.stringify(modelo)
    })
        .then(response => {
            $(".showSweetAlert").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) { // propiedad de gResponse en el Controller 

                swal("Listo!", "Su contraseña fue actualizada", "success")
                $("input.input-validar").val("");
            }
            else {
                swal("Lo sentimos", responseJson.mensaje, "error")
            }
        })
})