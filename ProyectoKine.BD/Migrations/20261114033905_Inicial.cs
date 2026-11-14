using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoKine.BD.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Paciente",
                columns: table => new
                {
                    IdPaciente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    Edad = table.Column<int>(type: "int", nullable: false),
                    Sexo = table.Column<string>(type: "varchar(20)", nullable: false),
                    Telefono = table.Column<string>(type: "varchar(20)", nullable: false),
                    Patologia = table.Column<string>(type: "varchar(200)", nullable: false),
                    TipoAtencion = table.Column<string>(type: "varchar(20)", nullable: false),
                    Direccion = table.Column<string>(type: "varchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paciente", x => x.IdPaciente);
                });

            migrationBuilder.CreateTable(
                name: "Turno",
                columns: table => new
                {
                    IdTurno = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Hora = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    Estado = table.Column<string>(type: "varchar(20)", nullable: false, defaultValue: "Pendiente"),
                    IdPaciente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turno", x => x.IdTurno);
                    table.ForeignKey(
                        name: "FK_Turno_Paciente",
                        column: x => x.IdPaciente,
                        principalTable: "Paciente",
                        principalColumn: "IdPaciente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Paciente_Telefono",
                table: "Paciente",
                column: "Telefono");

            migrationBuilder.CreateIndex(
                name: "IX_Turno_Fecha_Hora",
                table: "Turno",
                columns: new[] { "Fecha", "Hora" });

            migrationBuilder.CreateIndex(
                name: "IX_Turno_IdPaciente",
                table: "Turno",
                column: "IdPaciente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Turno");

            migrationBuilder.DropTable(
                name: "Paciente");
        }
    }
}
