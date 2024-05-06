using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralServices.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "link",
                columns: table => new
                {
                    recid = table.Column<Guid>(type: "uuid", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    savedateutc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    chatterid = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    fromChannel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_link", x => x.recid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "link");
        }
    }
}
