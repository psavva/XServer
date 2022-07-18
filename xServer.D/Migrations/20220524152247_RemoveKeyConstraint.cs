using Microsoft.EntityFrameworkCore.Migrations;

namespace x42.Migrations
{
    public partial class RemoveKeyConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_domain_KeyAddress",
                table: "domain");

            migrationBuilder.CreateIndex(
                name: "IX_domain_KeyAddress",
                table: "domain",
                column: "KeyAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_domain_KeyAddress",
                table: "domain");

            migrationBuilder.CreateIndex(
                name: "IX_domain_KeyAddress",
                table: "domain",
                column: "KeyAddress",
                unique: true);
        }
    }
}
