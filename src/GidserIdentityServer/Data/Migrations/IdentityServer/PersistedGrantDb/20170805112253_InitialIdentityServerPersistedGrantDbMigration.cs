using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidserIdentityServer.Data.Migrations.IdentityServer.PersistedGrantDb
{
    public partial class InitialIdentityServerPersistedGrantDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "PersistedGrants",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 200, nullable: true),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
					CreationTime = table.Column<DateTime>(nullable: true),
					Data = table.Column<string>(maxLength: 5000, nullable: false),
					Expiration = table.Column<DateTime>(nullable: true),
					SubjectId = table.Column<string>(maxLength: 200, nullable: true),
					Type = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersistedGrants", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersistedGrants");
        }
    }
}
