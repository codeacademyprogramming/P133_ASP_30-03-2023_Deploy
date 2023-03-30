using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P133FirstDbMigrationConnection.Migrations
{
    public partial class UpdatedGroupsTableAddedStudentCountColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "StudentCount",
                table: "Groups",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentCount",
                table: "Groups");
        }
    }
}
