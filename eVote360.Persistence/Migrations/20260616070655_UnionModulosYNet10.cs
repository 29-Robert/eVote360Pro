using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVote360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UnionModulosYNet10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartidoPoliticoId = table.Column<int>(type: "int", nullable: false),
                    PuestoElectivoId = table.Column<int>(type: "int", nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidatos_PartidosPoliticos_PartidoPoliticoId",
                        column: x => x.PartidoPoliticoId,
                        principalTable: "PartidosPoliticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Candidatos_PuestosElectivos_PuestoElectivoId",
                        column: x => x.PuestoElectivoId,
                        principalTable: "PuestosElectivos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesAlianzas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartidoSolicitanteId = table.Column<int>(type: "int", nullable: false),
                    PartidoReceptorId = table.Column<int>(type: "int", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesAlianzas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesAlianzas_PartidosPoliticos_PartidoReceptorId",
                        column: x => x.PartidoReceptorId,
                        principalTable: "PartidosPoliticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesAlianzas_PartidosPoliticos_PartidoSolicitanteId",
                        column: x => x.PartidoSolicitanteId,
                        principalTable: "PartidosPoliticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CandidatosPuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidatoId = table.Column<int>(type: "int", nullable: false),
                    PuestoElectivoId = table.Column<int>(type: "int", nullable: false),
                    PartidoPoliticoId = table.Column<int>(type: "int", nullable: false),
                    EsAliado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatosPuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatosPuestos_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidatosPuestos_PartidosPoliticos_PartidoPoliticoId",
                        column: x => x.PartidoPoliticoId,
                        principalTable: "PartidosPoliticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidatosPuestos_PuestosElectivos_PuestoElectivoId",
                        column: x => x.PuestoElectivoId,
                        principalTable: "PuestosElectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_PartidoPoliticoId",
                table: "Candidatos",
                column: "PartidoPoliticoId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_PuestoElectivoId",
                table: "Candidatos",
                column: "PuestoElectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatosPuestos_CandidatoId",
                table: "CandidatosPuestos",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatosPuestos_PartidoPoliticoId",
                table: "CandidatosPuestos",
                column: "PartidoPoliticoId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatosPuestos_PuestoElectivoId",
                table: "CandidatosPuestos",
                column: "PuestoElectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAlianzas_PartidoReceptorId",
                table: "SolicitudesAlianzas",
                column: "PartidoReceptorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAlianzas_PartidoSolicitanteId",
                table: "SolicitudesAlianzas",
                column: "PartidoSolicitanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidatosPuestos");

            migrationBuilder.DropTable(
                name: "SolicitudesAlianzas");

            migrationBuilder.DropTable(
                name: "Candidatos");
        }
    }
}
