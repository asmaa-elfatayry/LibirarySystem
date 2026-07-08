using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class handleDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Categories_CategoryId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_MemberId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_BookCopies_BookCopyId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_MemberId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Books_BookId",
                table: "Reservations");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Publishers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Loans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Fines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "LoanId1",
                table: "Fines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "BookCopies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ApplicationUserId",
                table: "Reservations",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ApplicationUserId",
                table: "Loans",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_LoanId1",
                table: "Fines",
                column: "LoanId1",
                unique: true,
                filter: "[LoanId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Categories_CategoryId",
                table: "Books",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_Loans_LoanId1",
                table: "Fines",
                column: "LoanId1",
                principalTable: "Loans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_ApplicationUserId",
                table: "Loans",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_MemberId",
                table: "Loans",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_BookCopies_BookCopyId",
                table: "Loans",
                column: "BookCopyId",
                principalTable: "BookCopies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_ApplicationUserId",
                table: "Reservations",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_MemberId",
                table: "Reservations",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Books_BookId",
                table: "Reservations",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Categories_CategoryId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Fines_Loans_LoanId1",
                table: "Fines");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_ApplicationUserId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_MemberId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_BookCopies_BookCopyId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_ApplicationUserId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_MemberId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Books_BookId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ApplicationUserId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Loans_ApplicationUserId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Fines_LoanId1",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "LoanId1",
                table: "Fines");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Publishers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Fines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "BookCopies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByName",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Categories_CategoryId",
                table: "Books",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_MemberId",
                table: "Loans",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_BookCopies_BookCopyId",
                table: "Loans",
                column: "BookCopyId",
                principalTable: "BookCopies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_MemberId",
                table: "Reservations",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Books_BookId",
                table: "Reservations",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
