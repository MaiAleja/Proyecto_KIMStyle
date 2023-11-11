$(document).ready(function () {
    // Botón: Agregar Clase
    $("#link-perfil").click(function () {
        $("#carta-perfil").removeClass("esconder-carta");
        $("#carta-perfil").addClass("mostrar-carta"); // Agregar la clase "mi-clase" al div
        $("#carta-perfil").focus();
          
    });
    $("#link-carrito").click(function () {
        $("#carta-carrito").removeClass("esconder-carta");
        $("#carta-carrito").addClass("mostrar-carta"); // Agregar la clase "mi-clase" al div
        $("#carta-carrito").focus();

    });

    // Div: Evento al perder el enfoque
    $(document).on("click", function (e) {
        var div = $("#carta-perfil");
        var boton = $("#link-perfil");
        if (!div.is(e.target) && !boton.is(e.target) && div.has(e.target).length === 0) {
            div.removeClass("mostrar-carta");
            div.addClass("esconder-carta");
        }
        div = $("#carta-carrito");
        boton = $("#link-carrito");
        if (!div.is(e.target) && !boton.is(e.target) && div.has(e.target).length === 0) {
            div.removeClass("mostrar-carta");
            div.addClass("esconder-carta");
        }
    });
});