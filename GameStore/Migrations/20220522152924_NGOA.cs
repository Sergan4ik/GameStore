using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Migrations
{
    public partial class NGOA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Game_Studios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Studio_Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game_Studios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Birth_date = table.Column<DateTime>(type: "date", nullable: true),
                    Balance = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Game_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Game_Studio_Id = table.Column<int>(type: "int", nullable: true),
                    Age_permission = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "money", nullable: true, defaultValueSql: "((0))"),
                    Genre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Game_Id);
                    table.ForeignKey(
                        name: "FK_Games_Game_Studios",
                        column: x => x.Game_Studio_Id,
                        principalTable: "Game_Studios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Game_copies",
                columns: table => new
                {
                    Copy_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<int>(type: "int", nullable: true),
                    Game_Id = table.Column<int>(type: "int", nullable: true),
                    Buy_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game_copies", x => x.Copy_Id);
                    table.ForeignKey(
                        name: "FK_Game_copies_Games",
                        column: x => x.Game_Id,
                        principalTable: "Games",
                        principalColumn: "Game_Id");
                    table.ForeignKey(
                        name: "FK_Game_copies_Users",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Game = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    Rarity = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Games",
                        column: x => x.Game,
                        principalTable: "Games",
                        principalColumn: "Game_Id");
                });

            migrationBuilder.CreateTable(
                name: "Promocodes",
                columns: table => new
                {
                    Promocode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Game = table.Column<int>(type: "int", nullable: true),
                    Discount = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promocodes", x => x.Promocode);
                    table.ForeignKey(
                        name: "FK_Promocodes_Games",
                        column: x => x.Game,
                        principalTable: "Games",
                        principalColumn: "Game_Id");
                });

            migrationBuilder.CreateTable(
                name: "Items_in_inventories",
                columns: table => new
                {
                    Item_copy_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Source_item_Id = table.Column<int>(type: "int", nullable: true),
                    Owner_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items_in_inventories", x => x.Item_copy_Id);
                    table.ForeignKey(
                        name: "FK_Items_in_inventories_Items",
                        column: x => x.Source_item_Id,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_in_inventories_Users",
                        column: x => x.Owner_id,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Game_copies_Game_Id",
                table: "Game_copies",
                column: "Game_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Game_copies_User_Id",
                table: "Game_copies",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Game_Studio_Id",
                table: "Games",
                column: "Game_Studio_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Game",
                table: "Items",
                column: "Game");

            migrationBuilder.CreateIndex(
                name: "IX_Items_in_inventories_Owner_id",
                table: "Items_in_inventories",
                column: "Owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_Items_in_inventories_Source_item_Id",
                table: "Items_in_inventories",
                column: "Source_item_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Promocodes_Game",
                table: "Promocodes",
                column: "Game");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Game_copies");

            migrationBuilder.DropTable(
                name: "Items_in_inventories");

            migrationBuilder.DropTable(
                name: "Promocodes");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Game_Studios");
        }
    }
}
