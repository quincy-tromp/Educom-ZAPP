using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamedTaskModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentTask_CareTask_TaskId",
                table: "AppointmentTask");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTask_CareTask_TaskId",
                table: "CustomerTask");

            migrationBuilder.DropTable(
                name: "CareTask");

            migrationBuilder.CreateTable(
                name: "TaskItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItem", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentTask_TaskItem_TaskId",
                table: "AppointmentTask",
                column: "TaskId",
                principalTable: "TaskItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTask_TaskItem_TaskId",
                table: "CustomerTask",
                column: "TaskId",
                principalTable: "TaskItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentTask_TaskItem_TaskId",
                table: "AppointmentTask");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTask_TaskItem_TaskId",
                table: "CustomerTask");

            migrationBuilder.DropTable(
                name: "TaskItem");

            migrationBuilder.CreateTable(
                name: "CareTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareTask", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentTask_CareTask_TaskId",
                table: "AppointmentTask",
                column: "TaskId",
                principalTable: "CareTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTask_CareTask_TaskId",
                table: "CustomerTask",
                column: "TaskId",
                principalTable: "CareTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
