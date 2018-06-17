namespace Delicious.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeIdtoINT : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Recipes", "Category_Id", "dbo.Categories");
            DropIndex("dbo.Recipes", new[] { "Category_Id" });
            DropPrimaryKey("dbo.Categories");
            AlterColumn("dbo.Categories", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Recipes", "Category_Id", c => c.Int());
            AddPrimaryKey("dbo.Categories", "Id");
            CreateIndex("dbo.Recipes", "Category_Id");
            AddForeignKey("dbo.Recipes", "Category_Id", "dbo.Categories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recipes", "Category_Id", "dbo.Categories");
            DropIndex("dbo.Recipes", new[] { "Category_Id" });
            DropPrimaryKey("dbo.Categories");
            AlterColumn("dbo.Recipes", "Category_Id", c => c.Guid());
            AlterColumn("dbo.Categories", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Categories", "Id");
            CreateIndex("dbo.Recipes", "Category_Id");
            AddForeignKey("dbo.Recipes", "Category_Id", "dbo.Categories", "Id");
        }
    }
}
