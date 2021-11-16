namespace SkylineUploaderDomain.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeStatusFieldLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Folders", "Status", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Folders", "Status", c => c.String(maxLength: 1000));
        }
    }
}
