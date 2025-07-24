using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastTodo.Persistence.EF.Data.Todo.Migrations
{
    /// <inheritdoc />
    public partial class Remove_StartEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndDate",
                table: "Tasks",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDate",
                table: "Tasks",
                type: "datetimeoffset",
                nullable: true);
        }
    }
}
