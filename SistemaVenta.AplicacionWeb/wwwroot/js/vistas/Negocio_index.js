$(document).ready(function () {

    $(".card-body").LoadingOverlay("show");

    fetch("/Negocio/Obtener")
        .then(response => {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            console.log(responseJson)
            if (responseJson.estado) {
                const objeto = responseJson.objeto

                $("#txtNumeroDocumento").val(objeto.numeroDocumento)
                $("#txtRazonSocial").val(objeto.nombre)
                $("#txtCorreo").val(objeto.correo)
                $("#txtDireccion").val(objeto.direccion)
                $("#txTelefono").val(objeto.telefono)
                $("#txtImpuesto").val(objeto.porcentajeImpuesto)
                $("#txtSimboloMoneda").val(objeto.simboloMoneda)
                $("#imgLogo").attr("src", objeto.urlLogo)
            } else {
                swal("Lo sentimos", responseJson.mensaje, "error")
            }
        })
})

$("#btnGuardarCambios").click(function () {
    const inputs = $("input.input-validar").serializeArray();
    console.log(inputs); // Muestra todos los inputs serializados en la consola

    const inputsSinValor = inputs.filter((item) => item.value.trim() == ""); // devuelve el input sin valor

    if (inputsSinValor.length > 0) {
        const mensaje = `Debe completar el campo: "${inputsSinValor[0].name}"`
        toastr.warning("", mensaje)
        $(`input[name]="${inputsSinValor[0].name}"`).focus()
        return;
    }

    const modelo = {
        numeroDocumento: $("#txtNumeroDocumento").val(),
        nombre: $("#txtRazonSocial").val(),
        correo: $("#txtCorreo").val(),
        direccion: $("#txtDireccion").val(),
        telefono: $("#txTelefono").val(),
        porcentajeImpuesto: $("#txtImpuesto").val(),
        simboloMoneda: $("#txtSimboloMoneda").val(),
    }
    const inputLogo = document.getElementById("txtLogo")

    const formData = new FormData();

    formData.append("logo", inputLogo.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $(".card-body").LoadingOverlay("show");

    fetch("/Negocio/GuardarCambios", {
        method: "POST",
        body: formData
    })
        .then(response => {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            console.log(responseJson)
            if (responseJson.estado) {
                const objeto = responseJson.objeto

                $("#imgLogo").attr("src", objeto.urlLogo)

            } else {
                swal("Lo sentimos", responseJson.mensaje, "error")
            }
        })
})
