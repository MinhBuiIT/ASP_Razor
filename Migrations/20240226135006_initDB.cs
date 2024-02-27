using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Bogus;
using ASP_RazorWeb.Models;

#nullable disable

namespace ASP_RazorWeb.Migrations
{
    /// <inheritdoc />
    public partial class initDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.Id);
                });
                Randomizer.Seed = new Random(8675309);
                var faker = new Faker<Article>();
                faker.RuleFor(a => a.Title,f => f.Lorem.Sentence(5,5));
                faker.RuleFor(a => a.Created,f => f.Date.Between(new DateTime(2023,1,1),new DateTime(2024,1,1)));
                faker.RuleFor(a => a.Content,f => f.Lorem.Paragraphs(1,3));

                for (int i = 0; i < 150; i++)
                {
                    var article = faker.Generate();
                    migrationBuilder.InsertData(table: "articles", columns: new string[]{"Title","Created","Content"},values: new object[]{article.Title,article.Created,article.Content});
                }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articles");
        }
    }
}
