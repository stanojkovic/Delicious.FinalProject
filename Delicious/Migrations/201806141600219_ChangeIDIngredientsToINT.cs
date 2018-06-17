namespace Delicious.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeIDIngredientsToINT : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IngredientRecipes", "Ingredient_Id", "dbo.Ingredients");
            DropIndex("dbo.IngredientRecipes", new[] { "Ingredient_Id" });
            DropPrimaryKey("dbo.Ingredients");
            DropPrimaryKey("dbo.IngredientRecipes");
            AlterColumn("dbo.Ingredients", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.IngredientRecipes", "Ingredient_Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Ingredients", "Id");
            AddPrimaryKey("dbo.IngredientRecipes", new[] { "Ingredient_Id", "Recipe_Id" });
            CreateIndex("dbo.IngredientRecipes", "Ingredient_Id");
            AddForeignKey("dbo.IngredientRecipes", "Ingredient_Id", "dbo.Ingredients", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IngredientRecipes", "Ingredient_Id", "dbo.Ingredients");
            DropIndex("dbo.IngredientRecipes", new[] { "Ingredient_Id" });
            DropPrimaryKey("dbo.IngredientRecipes");
            DropPrimaryKey("dbo.Ingredients");
            AlterColumn("dbo.IngredientRecipes", "Ingredient_Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.Ingredients", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.IngredientRecipes", new[] { "Ingredient_Id", "Recipe_Id" });
            AddPrimaryKey("dbo.Ingredients", "Id");
            CreateIndex("dbo.IngredientRecipes", "Ingredient_Id");
            AddForeignKey("dbo.IngredientRecipes", "Ingredient_Id", "dbo.Ingredients", "Id", cascadeDelete: true);
        }
    }
}
