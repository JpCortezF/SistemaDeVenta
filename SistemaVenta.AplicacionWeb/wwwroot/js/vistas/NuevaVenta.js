

let impuesto = 0;

$(document).ready(function () {
    fetch("/Venta/ListaTipoDocumentoVenta")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboTipoDocumentoVenta").append(
                        $("<option>").val(item.idTipoDocumentoVenta).text(item.descripcion)
                    )
                })
            }
        })


    fetch("/Negocio/Obtener")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const obj = responseJson.objeto;

                $("#inputGroupSubTotal").text(`Sub total - ${obj.simboloMoneda}`)
                $("#inputGroupIGV").text(`IGV(${obj.porcentajeImpuesto}%) - ${obj.simboloMoneda}`)
                $("#inputGroupTotal").text(`Total - ${obj.simboloMoneda}`)
                impuesto = parseFloat(obj.porcentajeImpuesto)
            }
        })

    $("#cboBuscarProducto").select2({ // Inicializacion Select2
        ajax: {
            url: "/Venta/ObtenerProductos",
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            delay: 250,
            data: function (params) {
                return {
                    busqueda: params.term  // El parametro de ObtenerProductos(string busqueda)
                };
            },
            processResults: function (data) {
                return {
                    results: data.map((item) => ({
                        id: item.idProducto,
                        text: item.descripcion,

                        marca: item.marca,
                        categoria: item.nombreCategoria,
                        urlImagen: item.urlImagen,
                        precio: parseFloat(item.precio)
                    }))
                };
            }
        },
        language: "es",
        placeholder: 'Buscar producto...',
        minimumInputLength: 1,
        templateResult: formatRepo,
    });

});


function formatRepo(data) {
    if (data.loading) {
        return data.text;
    }
    var contenedor = $(
        `<table width= "100%">
            <tr>
                <td>
                   <img style="height:60px;width:60px;margin-right:10px" src="${data.urlImagen}"/>
                </td>
                <td>
                    <p style="font-weight: bolder;margin:2px">${data.marca}</p>
                    <p style="margin:2px;">${data.text}</p>
                </td>
            </tr>
        </table>`
    );
    return contenedor;
}


$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus();
})


let productosVenta = [];
$("#cboBuscarProducto").on("select2:select", function (e) {
    const data = e.params.data;

    let productoSeleccionado = productosVenta.filter(p => p.idProducto == data.id)
    if (productoSeleccionado.length > 0) {
        $("#cboBuscarProducto").val("").trigger("change")
        toastr.warning("", "El producto ya fue agregado")
        return false
    }

    swal({
        title: data.marca,
        text: data.text,
        imageUrl: data.urlImagen,
        type: "input",
        confirmButtonText: "Aceptar",
        showCancelButton: true,
        closeOnConfirm: false,
        inputPlaceholder: "Ingrese cantidad"
    },
        function (valor) {

        if (valor === false) return false;

        if (valor === "") {
            toastr.warning("", "Necesita ingresar la cantidad")
            return false;
        }

        if (isNaN(parseInt(valor))) {
            toastr.warning("", "Debe ingresar un numero")
            return false;
        }

        let producto = {
            idProducto: data.id,
            marcaProducto: data.marca,
            descripcionProducto: data.text,
            categoriaProducto: data.categoria,
            cantidad: parseInt(valor),
            precio: data.precio.toString(),
            total: (parseFloat(valor) * data.precio).toString()
        };

        productosVenta.push(producto)
        mostrarPreciosProductos()

        $("#cboBuscarProducto").val("").trigger("change")
        swal.close();
    })
})

function mostrarPreciosProductos() {
    let total = 0;
    let igv = 0;
    let subtotal = 0;
    let porcentaje = impuesto / 100;

    $("#tbProducto tbody").html("")
    productosVenta.forEach((item) => {

        total = total + parseFloat(item.total)

        $("#tbProducto tbody").append(
            $("<tr>").append(
                $("<td>").append(
                    $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
                        $("<i>").addClass("fas fa-trash-alt")
                    ).data("idProducto", item.idProducto)
                ),
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total)
            )
        )
    })

    subtotal = total / (1 + porcentaje);
    igv = total - subtotal;

    $("#txtSubTotal").val(subtotal.toFixed(2))
    $("#txtIGV").val(igv.toFixed(2))
    $("#txtTotal").val(total.toFixed(2))
}

$(document).on("click", "button.btn-eliminar", function () {
    const _idProducto = $(this).data("idProducto")

    productosVenta = productosVenta.filter(p => p.idProducto != _idProducto);

    mostrarPreciosProductos();
})


$("#btnTerminarVenta").click(function () {

    if (productosVenta.length < 1) {
        toastr.warning("", "Debe ingresar productos")
        return false;
    }

    const vmDetalleVenta = productosVenta;

    const venta = {
        idTipoDocumentoVenta: $("#cboTipoDocumentoVenta").val(),
        documentoCliente: $("#txtDocumentoCliente").val(),
        nombreCliente: $("#txtNombreCliente").val(),
        subtotal: $("#txtSubTotal").val(),
        impuestoTotal: $("#txtIGV").val(),
        total: $("#txtTotal").val(),
        detalleVenta: vmDetalleVenta
    }

    $("#btnTerminarVenta").LoadingOverlay("show");

    fetch("/Venta/RegistrarVenta", {
        method: "POST",
        headers: { "Content-Type": "application/json;" }, //charset=utf8"
        body: JSON.stringify(venta)
    })
        .then(response => {
            $("#btnTerminarVenta").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                productosVenta = [];
                mostrarPreciosProductos();

                $("#txtDocumentoCliente").val("")
                $("#txtNombreCliente").val("")
                $("#cboTipoDocumentoVenta").val($("#cboTipoDocumentoVenta option:first").val())

                swal("Registro!", `Número de venta: ${responseJson.objeto.numeroVenta}`, "success")
            } else {
                swal("Lo sentimos!", "No se pudo registrar la venta", "error")
            }
        })
})