const VISTA_BUSQUEDA = {
    busquedaFecha: () => {
        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").show()
        $(".busqueda-venta").hide()
    },
    busquedaVenta: () => {
        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").hide()
        $(".busqueda-venta").show()
    }
}

$(document).ready(function () {

    VISTA_BUSQUEDA["busquedaFecha"]() // Se ejecuta la funcion busquedaFecha

    $.datepicker.setDefaults($.datepicker.regional["es"])  // Datepicker(calendario)

    $("#txtFechaInicio").datepicker({dateFormat: "dd/mm/yy"})
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

})

$("#cboBuscarPor").change(function () {
    if ($("#cboBuscarPor").val() == "fecha") {
        VISTA_BUSQUEDA["busquedaFecha"]()
    } else {
        VISTA_BUSQUEDA["busquedaVenta"]()
    }
})


$("#btnBuscar").click(function () {

    if ($("#cboBuscarPor").val() == "fecha") {
        if ($("#txtFechaInicio").val().trim == "" || $("#txtFechaFin").val().trim == "") {
            toastr.warning("", "Debe ingresar un rango de fechas")
            return;
        }
    } else {
        if ($("#txtNumeroVenta").val().trim() == "") {
            toastr.warning("", "Debe ingresar un número de venta")
            return;
        }
    }

    let numeroVenta = $("#txtNumeroVenta").val()
    let fechaInicio = $("#txtFechaInicio").val()
    let fechaFin = $("#txtFechaFin").val()

    $(".card-body").find("div.row").LoadingOverlay("show");

    fetch(`/Venta/Historial?numeroVenta=${numeroVenta}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`)
        .then(response => {
            $(".card-body").find("div.row").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            $("#tbventa tbody").html("");

            if (responseJson.length > 0) {

                responseJson.forEach((venta) => {

                    $("#tbventa tbody").append(
                        $("<tr>").append(
                            $("<td>").text(venta.fechaRegistro),
                            $("<td>").text(venta.numeroVenta),
                            $("<td>").text(venta.tipoDocumentoVenta),
                            $("<td>").text(venta.documentoCliente),
                            $("<td>").text(venta.nombreCliente),
                            $("<td>").text(venta.total),
                            $("<td>").append(
                                $("<button>").addClass("btn btn-info btn-sm").append(
                                    $("<i>").addClass("fas fa-eye")
                                ).data("venta", venta)
                            )
                        )
                    )
                })
            }
        })
})

$("#tbventa tbody").on("click", ".btn-info", function () {

    let dataVenta = $(this).data("venta")

    $("#txtFechaRegistro").val(dataVenta.fechaRegistro)
    $("#txtNumVenta").val(dataVenta.numeroVenta)
    $("#txtUsuarioRegistro").val(dataVenta.usuario)
    $("#txtTipoDocumento").val(dataVenta.tipoDocumentoVenta)
    $("#txtDocumentoCliente").val(dataVenta.documentoCliente)
    $("#txtNombreCliente").val(dataVenta.nombreCliente)
    $("#txtSubTotal").val(dataVenta.subTotal)
    $("#txtIGV").val(dataVenta.impuestoTotal)
    $("#txtTotal").val(dataVenta.total)

    $("#tbProductos tbody").html(""); // limpiamos el body de una tabla

    dataVenta.detalleVenta.forEach((item) => {

        $("#tbProductos tbody").append(
            $("<tr>").append(
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total),
            )
        )
    })


    $("#linkImprimir").attr("href", `/Venta/MostrarPDFVenta?numeroVenta=${dataVenta.numeroVenta}`)
    $("#modalData").modal("show");
})