using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaissePoly.Migrations
{
    /// <inheritdoc />
    public partial class firstmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Famille",
                columns: table => new
                {
                    idF = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nomF = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Famille", x => x.idF);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    IdT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTicket = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModePaiement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.IdT);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateur",
                columns: table => new
                {
                    idU = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nomU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MP = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateur", x => x.idU);
                });

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    idA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    prixunitaire = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    quantiteStock = table.Column<int>(type: "int", nullable: false),
                    quantiteVente = table.Column<int>(type: "int", nullable: false),
                    codeabare = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    idF = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.idA);
                    table.ForeignKey(
                        name: "FK_Article_Famille_idF",
                        column: x => x.idF,
                        principalTable: "Famille",
                        principalColumn: "idF");
                });

            migrationBuilder.CreateTable(
                name: "Vente",
                columns: table => new
                {
                    IdV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdA = table.Column<int>(type: "int", nullable: false),
                    Quantite = table.Column<int>(type: "int", nullable: false),
                    PrixUnitaire = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ArticleidA = table.Column<int>(type: "int", nullable: false),
                    TicketIdT = table.Column<int>(type: "int", nullable: true),
                    UtilisateuridU = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vente", x => x.IdV);
                    table.ForeignKey(
                        name: "FK_Vente_Article_ArticleidA",
                        column: x => x.ArticleidA,
                        principalTable: "Article",
                        principalColumn: "idA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vente_Tickets_TicketIdT",
                        column: x => x.TicketIdT,
                        principalTable: "Tickets",
                        principalColumn: "IdT");
                    table.ForeignKey(
                        name: "FK_Vente_Utilisateur_UtilisateuridU",
                        column: x => x.UtilisateuridU,
                        principalTable: "Utilisateur",
                        principalColumn: "idU");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_idF",
                table: "Article",
                column: "idF");

            migrationBuilder.CreateIndex(
                name: "IX_Vente_ArticleidA",
                table: "Vente",
                column: "ArticleidA");

            migrationBuilder.CreateIndex(
                name: "IX_Vente_TicketIdT",
                table: "Vente",
                column: "TicketIdT");

            migrationBuilder.CreateIndex(
                name: "IX_Vente_UtilisateuridU",
                table: "Vente",
                column: "UtilisateuridU");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vente");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Utilisateur");

            migrationBuilder.DropTable(
                name: "Famille");
        }
    }
}
