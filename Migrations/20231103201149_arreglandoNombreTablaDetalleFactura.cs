using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIM_Style.Migrations
{
    /// <inheritdoc />
    public partial class arreglandoNombreTablaDetalleFactura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detalle_Factua_Factura_id_factura",
                table: "Detalle_Factua");

            migrationBuilder.DropForeignKey(
                name: "FK_Detalle_Factua_Producto_cod_producto",
                table: "Detalle_Factua");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Detalle_Factua",
                table: "Detalle_Factua");

            migrationBuilder.RenameTable(
                name: "Detalle_Factua",
                newName: "Detalle_Factura");

            migrationBuilder.RenameIndex(
                name: "IX_Detalle_Factua_id_factura",
                table: "Detalle_Factura",
                newName: "IX_Detalle_Factura_id_factura");

            migrationBuilder.RenameIndex(
                name: "IX_Detalle_Factua_cod_producto",
                table: "Detalle_Factura",
                newName: "IX_Detalle_Factura_cod_producto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Detalle_Factura",
                table: "Detalle_Factura",
                column: "id_detalle");

            migrationBuilder.AddForeignKey(
                name: "FK_Detalle_Factura_Factura_id_factura",
                table: "Detalle_Factura",
                column: "id_factura",
                principalTable: "Factura",
                principalColumn: "id_factura",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Detalle_Factura_Producto_cod_producto",
                table: "Detalle_Factura",
                column: "cod_producto",
                principalTable: "Producto",
                principalColumn: "cod_producto",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detalle_Factura_Factura_id_factura",
                table: "Detalle_Factura");

            migrationBuilder.DropForeignKey(
                name: "FK_Detalle_Factura_Producto_cod_producto",
                table: "Detalle_Factura");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Detalle_Factura",
                table: "Detalle_Factura");

            migrationBuilder.RenameTable(
                name: "Detalle_Factura",
                newName: "Detalle_Factua");

            migrationBuilder.RenameIndex(
                name: "IX_Detalle_Factura_id_factura",
                table: "Detalle_Factua",
                newName: "IX_Detalle_Factua_id_factura");

            migrationBuilder.RenameIndex(
                name: "IX_Detalle_Factura_cod_producto",
                table: "Detalle_Factua",
                newName: "IX_Detalle_Factua_cod_producto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Detalle_Factua",
                table: "Detalle_Factua",
                column: "id_detalle");

            migrationBuilder.AddForeignKey(
                name: "FK_Detalle_Factua_Factura_id_factura",
                table: "Detalle_Factua",
                column: "id_factura",
                principalTable: "Factura",
                principalColumn: "id_factura",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Detalle_Factua_Producto_cod_producto",
                table: "Detalle_Factua",
                column: "cod_producto",
                principalTable: "Producto",
                principalColumn: "cod_producto",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
