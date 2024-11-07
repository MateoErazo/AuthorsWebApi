using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorsWebApi.Migrations
{
    /// <inheritdoc />
    public partial class PublicationDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Publication_Date",
                table: "Books",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Publication_Date",
                table: "Books");
        }
    }
}
