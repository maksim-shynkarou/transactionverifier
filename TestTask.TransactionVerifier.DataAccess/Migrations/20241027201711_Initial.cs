using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TestTask.TransactionVerifier.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CsvFiles",
                columns: table => new
                {
                    Hash = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CsvFiles", x => x.Hash);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CsvTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CsvFileHash = table.Column<string>(type: "nvarchar(1000)", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CsvTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CsvTransactions_CsvFiles_CsvFileHash",
                        column: x => x.CsvFileHash,
                        principalTable: "CsvFiles",
                        principalColumn: "Hash",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CsvTransactions_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "Description", "ProcessedAt" },
                values: new object[,]
                {
                    { new Guid("02671829-c8cb-4546-b583-69d189a81f6c"), 750.45m, "Transaction 19", new DateTime(2024, 10, 8, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8374) },
                    { new Guid("033ca3aa-4e64-4d4e-b3fc-da4784600c5c"), 80.00m, "Transaction 11", new DateTime(2024, 10, 16, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8357) },
                    { new Guid("0423973f-1114-4f76-82ed-408b2313ae20"), 120.45m, "Transaction 6", new DateTime(2024, 10, 21, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8347) },
                    { new Guid("0e701516-0b85-432a-97ec-baa585d29066"), 90.50m, "Transaction 12", new DateTime(2024, 10, 15, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8359) },
                    { new Guid("235f54e3-8866-43b8-9aff-066affb04d66"), 180.60m, "Transaction 14", new DateTime(2024, 10, 13, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8364) },
                    { new Guid("43bcd9af-b72a-43c1-af9c-92ae1c10324e"), 275.80m, "Transaction 15", new DateTime(2024, 10, 12, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8367) },
                    { new Guid("57a68c5c-ca56-4bdf-b0c9-e90219476a78"), 480.75m, "Transaction 17", new DateTime(2024, 10, 10, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8371) },
                    { new Guid("5806af0f-5c58-44c7-8e49-e9527f89bef5"), 100.50m, "Transaction 1", new DateTime(2024, 10, 26, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8271) },
                    { new Guid("60d0b373-3ee1-417c-9cc1-2275462139cb"), 510.65m, "Transaction 9", new DateTime(2024, 10, 18, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8353) },
                    { new Guid("60e6c624-d15f-4d96-8548-0cccffd60ad2"), 620.85m, "Transaction 18", new DateTime(2024, 10, 9, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8372) },
                    { new Guid("70f9e02b-cf9c-41e1-9967-cbb318983f59"), 150.95m, "Transaction 8", new DateTime(2024, 10, 19, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8352) },
                    { new Guid("7479a297-1f1e-4a20-8f48-0afb8db63c8b"), 60.75m, "Transaction 10", new DateTime(2024, 10, 17, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8355) },
                    { new Guid("76991db6-df92-4b7b-aad5-7211062436cc"), 310.25m, "Transaction 16", new DateTime(2024, 10, 11, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8369) },
                    { new Guid("902dbdf1-05e4-4c6f-9e13-9700d4e877fd"), 130.00m, "Transaction 20", new DateTime(2024, 10, 7, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8376) },
                    { new Guid("90308387-efad-493e-aa33-8d456373a963"), 50.25m, "Transaction 3", new DateTime(2024, 10, 24, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8333) },
                    { new Guid("a93f92f7-fce4-42e8-95aa-192eba2e9fb3"), 450.00m, "Transaction 5", new DateTime(2024, 10, 22, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8337) },
                    { new Guid("bc872857-c1a2-4341-a700-0771a05eed6b"), 225.35m, "Transaction 13", new DateTime(2024, 10, 14, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8360) },
                    { new Guid("becf17a0-53cf-479a-bb81-b1badd3f33c8"), 300.00m, "Transaction 4", new DateTime(2024, 10, 23, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8335) },
                    { new Guid("df7b794e-8e46-4c9e-884f-53c25dc99620"), 320.85m, "Transaction 7", new DateTime(2024, 10, 20, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8350) },
                    { new Guid("ff70324b-b59c-44da-b2e8-c2af7ae14777"), 200.75m, "Transaction 2", new DateTime(2024, 10, 25, 21, 17, 11, 726, DateTimeKind.Local).AddTicks(8329) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CsvTransactions_CsvFileHash",
                table: "CsvTransactions",
                column: "CsvFileHash");

            migrationBuilder.CreateIndex(
                name: "IX_CsvTransactions_TransactionId",
                table: "CsvTransactions",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CsvTransactions");

            migrationBuilder.DropTable(
                name: "CsvFiles");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
