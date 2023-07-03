using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CUConnect.Api.Migrations.CUConnectDB
{
    public partial class reactionTypeRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReactinType",
                table: "Reactions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReactinType",
                table: "Reactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
