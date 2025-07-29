using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform_Learning_Test.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewBuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
        name: "Explanation",
        table: "AnswerOptions",
        type: "nvarchar(1000)",
        maxLength: 1000,
        nullable: true, 
        oldClrType: typeof(string),
        oldType: "nvarchar(1000)",
        oldMaxLength: 1000);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
        name: "Explanation",
        table: "AnswerOptions",
        type: "nvarchar(1000)",
        maxLength: 1000,
        nullable: false,
        defaultValue: "",
        oldClrType: typeof(string),
        oldType: "nvarchar(1000)",
        oldMaxLength: 1000,
        oldNullable: true);
        }
    }
}
