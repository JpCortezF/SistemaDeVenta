
let tablaData;


$(document).ready(function () {


    $.datepicker.setDefaults($.datepicker.regional["es"])  // Datepicker(calendario)

    $("#txtFechaInicio").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

    if (!$.fn.DataTable.isDataTable('#tbdata')) {
        tablaData = $('#tbdata').DataTable({  // se convierte #tbdata a DataTable
            responsive: true,
            "ajax": {            // ajax devuelve toda la data el usuario
                "url": '/Reporte/ReporteVenta?fechaInicio=12/07/2000&fechaFin=12/07/2000',
                "type": "GET",
                "datatype": "json"
            },
            "columns": [
                { "data": "fechaRegistro" },
                { "data": "numeroVenta" },
                { "data": "tipoDocumento" },
                { "data": "nombreCliente" },
                { "data": "subTotalVenta" },
                { "data": "impuestoTotalVenta" },
                { "data": "totalVenta" },
                { "data": "producto" },
                { "data": "cantidad" },
                { "data": "precio" },
                { "data": "total" },
            ],
            order: [[0, "desc"]],
            dom: "Bfrtip",
            buttons: [
                {
                    text: 'Exportar Excel',
                    extend: 'excelHtml5',
                    title: '',
                    filename: 'Reporte Venta',
                }, 'pageLength'
            ],
            language: {  // url externa que envia la datatable convertida en español
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });
    }
})

$("#btnBuscar").click(function () {

    if ($("#txtFechaInicio").val().trim == "" || $("#txtFechaFin").val().trim == "")
    {
        toastr.warning("", "Debe ingresar un rango de fechas")
        return;
    }

    let fechaInicio = $("#txtFechaInicio").val()
    let fechaFin = $("#txtFechaFin").val()

    let nuevaURL = `/Reporte/ReporteVenta?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`

    tablaData.ajax.url(nuevaURL).load();


})