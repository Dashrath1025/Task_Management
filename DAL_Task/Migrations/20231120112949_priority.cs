using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL_Task.Migrations
{
    public partial class priority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "medium");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tasks");
        }
    }
}
