$(document).ready(function () {
    var cod_producto = -1;
    $.post("/Cliente/AgregarAlCarrito", { cod_producto: cod_producto }, function (data) {
        $("#carrito_compra").html(data);
    });
});
$(document).on("click", ".boton-carrito", function () {
    var cod_producto = $(this).data("cod_producto");
    $.post("/Cliente/AgregarAlCarrito", { cod_producto: cod_producto }, function (data) {
        $("#carrito_compra").html(data);
    });
});