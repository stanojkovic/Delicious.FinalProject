namespace Delicious.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePlaceQuantity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Ingredients", "Quantity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Ingredients", "Quantity", c => c.Double(nullable: false));
        }
    }
}
