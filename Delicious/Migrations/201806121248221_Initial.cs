namespace Delicious.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CategoriesName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RecipeName = c.String(),
                        Description = c.String(),
                        ImageName = c.String(),
                        InputDate = c.DateTime(nullable: false),
                        Category_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.Ingredients",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IngredientName = c.String(),
                        Quantity = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IngredientRecipes",
                c => new
                    {
                        Ingredient_Id = c.Guid(nullable: false),
                        Recipe_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Ingredient_Id, t.Recipe_Id })
                .ForeignKey("dbo.Ingredients", t => t.Ingredient_Id, cascadeDelete: true)
                .ForeignKey("dbo.Recipes", t => t.Recipe_Id, cascadeDelete: true)
                .Index(t => t.Ingredient_Id)
                .Index(t => t.Recipe_Id);
            
            AddColumn("dbo.AspNetUsers", "Name", c => c.String());
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IngredientRecipes", "Recipe_Id", "dbo.Recipes");
            DropForeignKey("dbo.IngredientRecipes", "Ingredient_Id", "dbo.Ingredients");
            DropForeignKey("dbo.Recipes", "Category_Id", "dbo.Categories");
            DropIndex("dbo.IngredientRecipes", new[] { "Recipe_Id" });
            DropIndex("dbo.IngredientRecipes", new[] { "Ingredient_Id" });
            DropIndex("dbo.Recipes", new[] { "Category_Id" });
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "Name");
            DropTable("dbo.IngredientRecipes");
            DropTable("dbo.Ingredients");
            DropTable("dbo.Recipes");
            DropTable("dbo.Categories");
        }
    }
}
