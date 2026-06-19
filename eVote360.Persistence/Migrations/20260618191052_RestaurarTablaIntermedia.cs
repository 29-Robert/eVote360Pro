using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVote360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RestaurarTablaIntermedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidatos_PuestosElectivos_PuestoElectivoId",
                table: "Candidatos");

            migrationBuilder.AddColumn<bool>(
                name: "YaVoto",
                table: "Ciudadanos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "PuestoElectivoId",
                table: "Candidatos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Candidatos_PuestosElectivos_PuestoElectivoId",
                table: "Candidatos",
                column: "PuestoElectivoId",
                principalTable: "PuestosElectivos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidatos_PuestosElectivos_PuestoElectivoId",
                table: "Candidatos");

            migrationBuilder.DropColumn(
                name: "YaVoto",
                table: "Ciudadanos");

            migrationBuilder.AlterColumn<int>(
                name: "PuestoElectivoId",
                table: "Candidatos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidatos_PuestosElectivos_PuestoElectivoId",
                table: "Candidatos",
                column: "PuestoElectivoId",
                principalTable: "PuestosElectivos",
                principalColumn: "Id");
        }
    }
}
