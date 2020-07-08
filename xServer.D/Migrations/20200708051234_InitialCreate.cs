﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace x42.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "profile",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    KeyAddress = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    PriceLockId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "profilelock",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PriceLockId = table.Column<string>(nullable: true),
                    FeeAmount = table.Column<decimal>(nullable: false),
                    FeeAddress = table.Column<string>(nullable: true),
                    DestinationAmount = table.Column<decimal>(nullable: false),
                    DestinationAddress = table.Column<string>(nullable: true),
                    PriceLockSignature = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profilelock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "server",
                columns: table => new
                {
                    ProfileName = table.Column<string>(nullable: false),
                    PublicAddress = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server", x => x.ProfileName);
                });

            migrationBuilder.CreateTable(
                name: "servernode",
                columns: table => new
                {
                    ProfileName = table.Column<string>(nullable: false),
                    NetworkProtocol = table.Column<int>(nullable: false),
                    NetworkAddress = table.Column<string>(nullable: true),
                    NetworkPort = table.Column<long>(nullable: false),
                    Tier = table.Column<int>(nullable: false),
                    Signature = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    LastSeen = table.Column<DateTime>(nullable: false),
                    Priority = table.Column<long>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Relayed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servernode", x => x.ProfileName);
                });

            migrationBuilder.CreateIndex(
                name: "IX_profile_KeyAddress",
                table: "profile",
                column: "KeyAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_profile_Name",
                table: "profile",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_server_ProfileName",
                table: "server",
                column: "ProfileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_servernode_ProfileName",
                table: "servernode",
                column: "ProfileName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "profile");

            migrationBuilder.DropTable(
                name: "profilelock");

            migrationBuilder.DropTable(
                name: "server");

            migrationBuilder.DropTable(
                name: "servernode");
        }
    }
}