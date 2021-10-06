namespace SkylineUploaderDomain.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInEditModeAndDateUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Folders", "InEditMode", c => c.Boolean(nullable: true));
            AddColumn("dbo.Folders", "DateUpdated", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Folders", "DateUpdated");
            DropColumn("dbo.Folders", "InEditMode");
        }
    }
}
