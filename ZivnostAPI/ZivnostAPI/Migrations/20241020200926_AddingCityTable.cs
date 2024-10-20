using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZivnostAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddingCityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCompany",
                table: "Account",
                newName: "IsHybridAccount");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompanyAccount",
                table: "Account",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomerAccount",
                table: "Account",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PictureURL",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredAsCompanyAt",
                table: "Account",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredAsCustomerAt",
                table: "Account",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SureName",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    District_Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropColumn(
                name: "IsCompanyAccount",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "IsCustomerAccount",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PictureURL",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "RegisteredAsCompanyAt",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "RegisteredAsCustomerAt",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "SureName",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "IsHybridAccount",
                table: "Account",
                newName: "IsCompany");
        }
    }
}
