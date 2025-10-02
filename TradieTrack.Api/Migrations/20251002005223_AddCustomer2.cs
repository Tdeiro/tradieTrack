using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradieTrack.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomer2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Organizations_OrganizationId",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                table: "Customer");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Customers");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_OrganizationId_Phone",
                table: "Customers",
                newName: "IX_Customers_OrganizationId_Phone");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_OrganizationId_Email",
                table: "Customers",
                newName: "IX_Customers_OrganizationId_Email");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrganizationId1",
                table: "Users",
                column: "OrganizationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Organizations_OrganizationId",
                table: "Customers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Organizations_OrganizationId1",
                table: "Users",
                column: "OrganizationId1",
                principalTable: "Organizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Organizations_OrganizationId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Organizations_OrganizationId1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_OrganizationId1",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Customer");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_OrganizationId_Phone",
                table: "Customer",
                newName: "IX_Customer_OrganizationId_Phone");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_OrganizationId_Email",
                table: "Customer",
                newName: "IX_Customer_OrganizationId_Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                table: "Customer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Organizations_OrganizationId",
                table: "Customer",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
