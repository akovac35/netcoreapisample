using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.Oracle.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    FirstName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    LastName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RowVersion = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UniqueId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Consents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ConsentType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ValidFromUtc = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    ValidThroughUtc = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    Revoked = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RowVersion = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UniqueId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    AuthorId = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    Year = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Publisher = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DbComputedColumn = table.Column<int>(type: "NUMBER(10)", nullable: true, computedColumnSql: "\"RowVersion\" + 1"),
                    DbDefaultValueColumn = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RowVersion = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UniqueId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authors_UniqueId",
                table: "Authors",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_UniqueId",
                table: "Books",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consents_UniqueId",
                table: "Consents",
                column: "UniqueId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Consents");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
