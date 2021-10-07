using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace NineNineQuotes.Migrations
{
    public partial class AddTextSearchVectorColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Quotes",
                type: "tsvector",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_Episode_Character_QuoteText",
                table: "Quotes",
                columns: new[] { "Episode", "Character", "QuoteText" })
                .Annotation("Npgsql:TsVectorConfig", "english");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Quotes_Episode_Character_QuoteText",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "Quotes");
        }
    }
}
