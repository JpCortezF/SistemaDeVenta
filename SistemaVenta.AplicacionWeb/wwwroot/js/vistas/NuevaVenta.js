



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
            if (responseJson.estado)
            {
                const obj = responseJson.objeto;
                console.log(obj)

                $("#inputGroupSubTotal").text(`Sub total - ${obj.simboloMoneda}`)
                $("#inputGroupIGV").text(`IGV(${obj.porcentajeImpuesto}%) - ${obj.simboloMoneda}`)
                $("#inputGroupTotal").text(`Total - ${obj.simboloMoneda}`)
            }
        })

    fetch("/Productos/Lista")
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

    $("#cboBuscarProducto").select2({
        ajax: {
            url: "/Venta/ObtenerProductos",
            dataType: 'json',
            contentType: "application/json;",
            delay: 250,
            data: function (params) {
                return {
                    busqueda: params.term  // El parametro de ObtenerProductos(string busqueda)
                };
            },
            processResults: function (data) {

                return
                {
                    results: data.map((item) => (
                    {
                        id: item.idProducto,
                        text: item.descripcion,

                        marca: item.marca,
                        categoria: item.nombreCategoria,
                        urlImagen: item.urlImagen,
                        precio: parseFloat(item.precio)
                    }))
                };
            },
        },
        language: "es",
        placeholder: 'Buscar producto...',
        minimumInputLength: 1,
        templateResult: formatRepo,
    });

});

function formatRepo(data) {
    if (repo.loading) {
        return repo.text;
    }


}