using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfol.io.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTimelineEventDisplayOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "TimelineEvents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "TimelineEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
