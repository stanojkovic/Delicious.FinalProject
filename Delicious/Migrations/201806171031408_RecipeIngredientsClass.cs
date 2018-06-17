namespace Delicious.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecipeIngredientsClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecipeIngredients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitOfMeasure = c.String(),
                        Ingredient_Id = c.Int(),
                        Recipe_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ingredients", t => t.Ingredient_Id)
                .ForeignKey("dbo.Recipes", t => t.Recipe_Id)
                .Index(t => t.Ingredient_Id)
                .Index(t => t.Recipe_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecipeIngredients", "Recipe_Id", "dbo.Recipes");
            DropForeignKey("dbo.RecipeIngredients", "Ingredient_Id", "dbo.Ingredients");
            DropIndex("dbo.RecipeIngredients", new[] { "Recipe_Id" });
            DropIndex("dbo.RecipeIngredients", new[] { "Ingredient_Id" });
            DropTable("dbo.RecipeIngredients");
        }
    }
}
