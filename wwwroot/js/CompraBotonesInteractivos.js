$(document).ready(function () {
    $(".boton-menos").click(function () {
        var inputCantidad = $(this).next(".input-cantidad");
        var cantidad = parseInt(inputCantidad.val());
        var cod_producto = parseInt(inputCantidad.data("cod_producto"));
        if (cantidad > 1) {
            cantidad--;
            inputCantidad.val(cantidad);
        }
        $.ajax({
            url: '/Cliente/ActualizarTotalCompra',
            type: 'POST',
            data: { cod_producto: cod_producto, cantidad: cantidad },
            success: function (data) {
                var totalCompraFormateado = data.totalCompra.toLocaleString("es-CO", { style: "currency", currency: "COP" });
                $("#total-compra").text(totalCompraFormateado);
            },
            error: function () {
                alert('Hubo un error al actualizar la compra.');
            }
        });
    });

    $(".boton-mas").click(function () {
        var inputCantidad = $(this).prev(".input-cantidad");
        var cantidad = parseInt(inputCantidad.val());
        var max = parseInt(inputCantidad.data("max"));
        var cod_producto = parseInt(inputCantidad.data("cod_producto"));

        if (cantidad < max) {
            cantidad++;
            inputCantidad.val(cantidad);
        }
        $.ajax({
            url: '/Cliente/ActualizarTotalCompra',
            type: 'POST',
            data: { cod_producto: cod_producto, cantidad: cantidad },
            success: function (data) {
                var totalCompraFormateado = data.totalCompra.toLocaleString("es-CO", { style: "currency", currency: "COP" });
                $("#total-compra").text(totalCompraFormateado);
            },
            error: function () {
                alert('Hubo un error al actualizar la compra.');
            }
        });
    });
});