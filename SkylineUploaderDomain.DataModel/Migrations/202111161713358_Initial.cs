namespace SkylineUploaderDomain.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Folders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FolderId = c.Guid(nullable: false),
                        PortalId = c.Guid(nullable: false),
                        FolderName = c.String(maxLength: 1000),
                        Status = c.String(maxLength: 200),
                        Files = c.Int(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        SynchronizeFiles = c.Boolean(nullable: false),
                        DeleteAfterDays = c.Int(nullable: false),
                        HideOnOrder = c.Boolean(nullable: false),
                        DeleteAfterUpload = c.Boolean(nullable: false),
                        InEditMode = c.Boolean(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        FileType = c.String(maxLength: 1000),
                        Login_Id = c.Int(),
                        SourceFolder_Id = c.Int(),
                        UserLibrary_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Logins", t => t.Login_Id)
                .ForeignKey("dbo.SourceFolders", t => t.SourceFolder_Id)
                .ForeignKey("dbo.UserLibraries", t => t.UserLibrary_Id)
                .Index(t => t.Login_Id)
                .Index(t => t.SourceFolder_Id)
                .Index(t => t.UserLibrary_Id);
            
            CreateTable(
                "dbo.Logins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FolderId = c.Guid(nullable: false),
                        Username = c.String(maxLength: 1000),
                        Password = c.String(maxLength: 1000),
                        PortalUrl = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SourceFolders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FolderId = c.Guid(nullable: false),
                        FolderPath = c.String(maxLength: 1000),
                        Enabled = c.Boolean(nullable: false),
                        FileCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SynchronizedFiles",
                c => new
                    {
                        Id = c.Boolean(nullable: false),
                        FolderId = c.Guid(nullable: false),
                        DocumentName = c.String(maxLength: 1000),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        DateAccessed = c.DateTime(nullable: false),
                        Size = c.Int(nullable: false),
                        Folder_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Folders", t => t.Folder_Id)
                .Index(t => t.Folder_Id);
            
            CreateTable(
                "dbo.UserLibraries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FolderId = c.Guid(nullable: false),
                        Username = c.String(maxLength: 1000),
                        UserId = c.Guid(nullable: false),
                        UserEmail = c.String(maxLength: 1000),
                        LibraryName = c.String(maxLength: 1000),
                        LibraryId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Proxies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UseProxy = c.Boolean(nullable: false),
                        ProxyAddress = c.String(maxLength: 1000),
                        ProxyPort = c.Int(nullable: false),
                        ProxyUsername = c.String(maxLength: 1000),
                        ProxyPassword = c.String(maxLength: 1000),
                        ProxyDomain = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ServiceSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastUpdate = c.DateTime(nullable: false),
                        ServiceMessage = c.String(maxLength: 1000),
                        Progress = c.Int(nullable: false),
                        ProgressMaximum = c.Int(nullable: false),
                        Running = c.Boolean(nullable: false),
                        Uploading = c.Boolean(nullable: false),
                        Transferring = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Folders", "UserLibrary_Id", "dbo.UserLibraries");
            DropForeignKey("dbo.SynchronizedFiles", "Folder_Id", "dbo.Folders");
            DropForeignKey("dbo.Folders", "SourceFolder_Id", "dbo.SourceFolders");
            DropForeignKey("dbo.Folders", "Login_Id", "dbo.Logins");
            DropIndex("dbo.SynchronizedFiles", new[] { "Folder_Id" });
            DropIndex("dbo.Folders", new[] { "UserLibrary_Id" });
            DropIndex("dbo.Folders", new[] { "SourceFolder_Id" });
            DropIndex("dbo.Folders", new[] { "Login_Id" });
            DropTable("dbo.ServiceSettings");
            DropTable("dbo.Proxies");
            DropTable("dbo.UserLibraries");
            DropTable("dbo.SynchronizedFiles");
            DropTable("dbo.SourceFolders");
            DropTable("dbo.Logins");
            DropTable("dbo.Folders");
        }
    }
}
