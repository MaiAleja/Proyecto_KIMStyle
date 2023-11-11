using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIM_Style.Migrations
{
    /// <inheritdoc />
    public partial class agregaCalificacionPromedio_TablaProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auditoria",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tabla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    valor_anterior = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    valor_nuevo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sql = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditoria", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Color",
                columns: table => new
                {
                    id_color = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    color_nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Color", x => x.id_color);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    id_rol = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_rol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "Tipo_Marca",
                columns: table => new
                {
                    id_marca = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipo_Marca", x => x.id_marca);
                });

            migrationBuilder.CreateTable(
                name: "Tipo_Prenda",
                columns: table => new
                {
                    id_prenda = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipo_Prenda", x => x.id_prenda);
                });

            migrationBuilder.CreateTable(
                name: "Tipo_Talla",
                columns: table => new
                {
                    id_talla = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    talla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipo_Talla", x => x.id_talla);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    cedula = table.Column<int>(type: "int", nullable: false),
                    nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    genero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    constrasena = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    rol_usuario = table.Column<byte>(type: "tinyint", nullable: false),
                    FotoPerfil = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.cedula);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol_rol_usuario",
                        column: x => x.rol_usuario,
                        principalTable: "Rol",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    cod_producto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    temporada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    genero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    calificacion_promedio = table.Column<int>(type: "int", nullable: false),
                    Imagen = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    valor = table.Column<decimal>(type: "decimal(11,2)", nullable: false),
                    id_color = table.Column<int>(type: "int", nullable: false),
                    id_tipo_prenda = table.Column<byte>(type: "tinyint", nullable: false),
                    marca = table.Column<byte>(type: "tinyint", nullable: false),
                    talla = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.cod_producto);
                    table.ForeignKey(
                        name: "FK_Producto_Color_id_color",
                        column: x => x.id_color,
                        principalTable: "Color",
                        principalColumn: "id_color",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Producto_Tipo_Marca_marca",
                        column: x => x.marca,
                        principalTable: "Tipo_Marca",
                        principalColumn: "id_marca",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Producto_Tipo_Prenda_id_tipo_prenda",
                        column: x => x.id_tipo_prenda,
                        principalTable: "Tipo_Prenda",
                        principalColumn: "id_prenda",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Producto_Tipo_Talla_talla",
                        column: x => x.talla,
                        principalTable: "Tipo_Talla",
                        principalColumn: "id_talla",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Factura",
                columns: table => new
                {
                    id_factura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    valor_total = table.Column<decimal>(type: "decimal(11,2)", nullable: false),
                    tipo_transaccion = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    estado_venta = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factura", x => x.id_factura);
                    table.ForeignKey(
                        name: "FK_Factura_Usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "cedula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Calificacion",
                columns: table => new
                {
                    cod_producto = table.Column<int>(type: "int", nullable: false),
                    ced_usuario = table.Column<int>(type: "int", nullable: false),
                    id_calificacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    calificacion = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calificacion", x => x.id_calificacion);
                    table.ForeignKey(
                        name: "FK_Calificacion_Producto_cod_producto",
                        column: x => x.cod_producto,
                        principalTable: "Producto",
                        principalColumn: "cod_producto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Calificacion_Usuario_ced_usuario",
                        column: x => x.ced_usuario,
                        principalTable: "Usuario",
                        principalColumn: "cedula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Detalle_Factua",
                columns: table => new
                {
                    id_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    total_producto = table.Column<decimal>(type: "decimal(11,2)", nullable: false),
                    id_factura = table.Column<int>(type: "int", nullable: false),
                    cod_producto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detalle_Factua", x => x.id_detalle);
                    table.ForeignKey(
                        name: "FK_Detalle_Factua_Factura_id_factura",
                        column: x => x.id_factura,
                        principalTable: "Factura",
                        principalColumn: "id_factura",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Detalle_Factua_Producto_cod_producto",
                        column: x => x.cod_producto,
                        principalTable: "Producto",
                        principalColumn: "cod_producto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calificacion_ced_usuario",
                table: "Calificacion",
                column: "ced_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Calificacion_cod_producto",
                table: "Calificacion",
                column: "cod_producto");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Factua_cod_producto",
                table: "Detalle_Factua",
                column: "cod_producto");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Factua_id_factura",
                table: "Detalle_Factua",
                column: "id_factura");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_id_usuario",
                table: "Factura",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_id_color",
                table: "Producto",
                column: "id_color");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_id_tipo_prenda",
                table: "Producto",
                column: "id_tipo_prenda");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_marca",
                table: "Producto",
                column: "marca");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_talla",
                table: "Producto",
                column: "talla");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_rol_usuario",
                table: "Usuario",
                column: "rol_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditoria");

            migrationBuilder.DropTable(
                name: "Calificacion");

            migrationBuilder.DropTable(
                name: "Detalle_Factua");

            migrationBuilder.DropTable(
                name: "Factura");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Color");

            migrationBuilder.DropTable(
                name: "Tipo_Marca");

            migrationBuilder.DropTable(
                name: "Tipo_Prenda");

            migrationBuilder.DropTable(
                name: "Tipo_Talla");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
