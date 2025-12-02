using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bolonotoproxy.Migrations
{
    /// <inheritdoc />
    public partial class FixTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "image",
                table: "Students",
                newName: "signupID");

            migrationBuilder.AddColumn<string>(
                name: "imageAddress",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_signupID",
                table: "Students",
                column: "signupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_register_signupID",
                table: "Students",
                column: "signupID",
                principalTable: "register",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_register_signupID",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_signupID",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "imageAddress",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "signupID",
                table: "Students",
                newName: "image");
        }
    }
}
