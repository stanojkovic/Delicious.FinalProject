namespace Delicious.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUnitEnum : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RecipeIngredients", "UnitOfMeasure", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RecipeIngredients", "UnitOfMeasure", c => c.String());
        }
    }
}
