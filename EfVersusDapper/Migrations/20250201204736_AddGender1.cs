using EfVersusDapper.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfVersusDapper.Migrations
{
    /// <inheritdoc />
    public partial class AddGender1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:Gender", "male,female");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Customers",
                type: "\"Gender\"",
                nullable: false,
                defaultValueSql: "'male'::\"Gender\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Customers");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:Gender", "male,female");
        }
    }
}
