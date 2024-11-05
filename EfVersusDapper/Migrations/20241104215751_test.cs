using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfVersusDapper.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.Sql("""
                                 DELETE FROM "Customers" WHERE "Name" = 'Angie Bergnaum'
                                 """);

            migrationBuilder.Sql("""
                                 DELETE FROM "Customers" WHERE "Name" in (SELECT "Name" FROM "Customers"
                                 GROUP BY "Name"
                                 HAVING COUNT(*) > 1)
                                 """);

            
            migrationBuilder.CreateIndex(
                name: "IX_Customers_Name",
                table: "Customers",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Name",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Orders");
        }
    }
}
