using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P133Allup.Migrations
{
    public partial class UpdatedCategoriesTable_V3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "Categories");
        }
    }
}
