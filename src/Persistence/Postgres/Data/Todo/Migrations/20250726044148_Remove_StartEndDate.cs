using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastTodo.Persistence.Postgres.Data.Todo.Migrations
{
    /// <inheritdoc />
    public partial class Remove_StartEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "todo",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "todo",
                table: "Tasks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedAt",
                schema: "todo",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "todo",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "todo",
                table: "Tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "todo",
                table: "Tasks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "todo",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "todo",
                table: "Tasks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedAt",
                schema: "todo",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "todo",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "todo",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "todo",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
