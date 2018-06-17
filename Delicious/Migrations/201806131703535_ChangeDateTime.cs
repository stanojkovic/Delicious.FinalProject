namespace Delicious.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDateTime : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Recipes", "InputDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "InputDate", c => c.DateTime(nullable: false));
        }
    }
}
