using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FifaTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviorToCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchTeams_Users_UserId",
                table: "MatchTeams");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchTeams_Users_UserId",
                table: "MatchTeams",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchTeams_Users_UserId",
                table: "MatchTeams");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchTeams_Users_UserId",
                table: "MatchTeams",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
