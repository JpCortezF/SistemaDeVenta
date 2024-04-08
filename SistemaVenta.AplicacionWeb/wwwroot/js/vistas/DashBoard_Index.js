
$(document).ready(function () {

    $("div.container-fluid").LoadingOverlay("show");

    fetch("/DashBoard/ObtenerResumen")
        .then(response => {
            $("div.container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            if (responseJson.estado) {

                let data = responseJson.objeto

                $("#totalVenta").text(data.totalVentas)
                $("#totalIngresos").text(data.totalIngresos)
                $("#totalProductos").text(data.totalProductos)
                $("#totalCategorias").text(data.totalCategorias)

                // Gráfico de ventas ult. 7 días

                let barchart_labels;
                let barchart_data;

                if (data.ventasUltimaSemana.length > 0) {
                    barchart_labels = data.ventasUltimaSemana.map((item) => { return item.fecha })
                    barchart_data = data.ventasUltimaSemana.map((item) => { return item.total })
                } else {
                    barchart_labels = ["No hay resultados"]
                    barchart_data = [0]
                }

                // Gráfico PIE para los productos

                let piechart_labels;
                let piechart_data;

                if (data.productosTopUltimaSemana.length > 0) {
                    piechart_labels = data.productosTopUltimaSemana.map((item) => { return item.producto })
                    piechart_data = data.productosTopUltimaSemana.map((item) => { return item.cantidad })
                } else {
                    piechart_labels = ["No hay resultados"]
                    piechart_data = [0]
                }

                // Bar Chart Example
                let controlVenta = document.getElementById("chartVentas");
                let myBarChart = new Chart(controlVenta, {
                    type: 'bar',
                    data: {
                        labels: barchart_labels,
                        datasets: [{
                            label: "Cantidad",
                            backgroundColor: "#4e73df",
                            hoverBackgroundColor: "#2e59d9",
                            borderColor: "#4e73df",
                            data: barchart_data,
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: {
                            display: false
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 50,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 5
                                }
                            }],
                        },
                    }
                });


                // Pie Chart Example
                let controlProducto = document.getElementById("chartProductos");
                let myPieChart = new Chart(controlProducto, {
                    type: 'doughnut',
                    data: {
                        labels: piechart_labels,
                        datasets: [{
                            data: piechart_data,
                            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', "#FF785B"],
                            hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf', "#FF5733"],
                            hoverBorderColor: "rgba(234, 236, 244, 1)",
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        tooltips: {
                            backgroundColor: "rgb(255,255,255)",
                            bodyFontColor: "#858796",
                            borderColor: '#dddfeb',
                            borderWidth: 1,
                            xPadding: 15,
                            yPadding: 15,
                            displayColors: false,
                            caretPadding: 10,
                        },
                        legend: {
                            display: true
                        },
                        cutoutPercentage: 80,
                    },
                });


            } 
        })
})