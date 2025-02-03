using EfVersusDapper.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfVersusDapper.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderv9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:Gender", "Female,Male");

            migrationBuilder.AddColumn<Gender>(
                name: "Gender",
                table: "Customers",
                type: "\"Gender\"",
                nullable: false,
                defaultValue: Gender.Male);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Customers");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:Gender", "Female,Male");
        }
    }
}
