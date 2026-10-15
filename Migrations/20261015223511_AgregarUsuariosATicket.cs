using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskWeb.Migrations
{
    /// <inheritdoc />
    public partial class AgregarUsuariosATicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Ticket",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Ticket",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedToId",
                table: "Ticket",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "Ticket",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedById",
                table: "Ticket",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Ticket",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Ticket",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "Ticket",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolvedById",
                table: "Ticket",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketStatusId",
                table: "Ticket",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_AssignedToId",
                table: "Ticket",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ClosedById",
                table: "Ticket",
                column: "ClosedById");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CreatedById",
                table: "Ticket",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ResolvedById",
                table: "Ticket",
                column: "ResolvedById");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TicketStatusId",
                table: "Ticket",
                column: "TicketStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_AspNetUsers_AssignedToId",
                table: "Ticket",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_AspNetUsers_ClosedById",
                table: "Ticket",
                column: "ClosedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_AspNetUsers_CreatedById",
                table: "Ticket",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_AspNetUsers_ResolvedById",
                table: "Ticket",
                column: "ResolvedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_TicketStatus_TicketStatusId",
                table: "Ticket",
                column: "TicketStatusId",
                principalTable: "TicketStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_AspNetUsers_AssignedToId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_AspNetUsers_ClosedById",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_AspNetUsers_CreatedById",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_AspNetUsers_ResolvedById",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_TicketStatus_TicketStatusId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_AssignedToId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_ClosedById",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_CreatedById",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_ResolvedById",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_TicketStatusId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "ClosedById",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "ResolvedById",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "TicketStatusId",
                table: "Ticket");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Ticket",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Ticket",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
