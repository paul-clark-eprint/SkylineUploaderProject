namespace SkylineUploaderDomain.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FolderId = c.Guid(nullable: false),
                        FileType = c.String(),
                        Folder_Id = c.Int(),
                        SourceFolder_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Folders", t => t.Folder_Id)
                .ForeignKey("dbo.SourceFolders", t => t.SourceFolder_Id)
                .Index(t => t.Folder_Id)
                .Index(t => t.SourceFolder_Id);
            
            CreateTable(
                "dbo.Folders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FolderId = c.Guid(nullable: false),
                        PortalId = c.Guid(nullable: false),
                        FolderName = c.String(),
                        Status = c.Int(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        SynchronizeFiles = c.Boolean(nullable: false),
                        DeleteAfterDays = c.Int(nullable: false),
                        HideOnOrder = c.Boolean(nullable: false),
                        DeleteAfterUpload = c.Boolean(nullable: false),
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
                        Username = c.String(),
                        Password = c.String(),
                        PortalUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SourceFolders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FolderId = c.Guid(nullable: false),
                        FolderPath = c.String(),
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
                        DocumentName = c.String(),
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
                        Username = c.String(),
                        UserId = c.Guid(nullable: false),
                        UserEmail = c.String(),
                        LibraryName = c.String(),
                        LibraryId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Proxies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UseProxy = c.Boolean(nullable: false),
                        ProxyAddress = c.String(),
                        ProxyPort = c.Int(nullable: false),
                        ProxyUsername = c.String(),
                        ProxyPassword = c.String(),
                        ProxyDomain = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Folders", "UserLibrary_Id", "dbo.UserLibraries");
            DropForeignKey("dbo.SynchronizedFiles", "Folder_Id", "dbo.Folders");
            DropForeignKey("dbo.Folders", "SourceFolder_Id", "dbo.SourceFolders");
            DropForeignKey("dbo.DocumentTypes", "SourceFolder_Id", "dbo.SourceFolders");
            DropForeignKey("dbo.Folders", "Login_Id", "dbo.Logins");
            DropForeignKey("dbo.DocumentTypes", "Folder_Id", "dbo.Folders");
            DropIndex("dbo.SynchronizedFiles", new[] { "Folder_Id" });
            DropIndex("dbo.Folders", new[] { "UserLibrary_Id" });
            DropIndex("dbo.Folders", new[] { "SourceFolder_Id" });
            DropIndex("dbo.Folders", new[] { "Login_Id" });
            DropIndex("dbo.DocumentTypes", new[] { "SourceFolder_Id" });
            DropIndex("dbo.DocumentTypes", new[] { "Folder_Id" });
            DropTable("dbo.Proxies");
            DropTable("dbo.UserLibraries");
            DropTable("dbo.SynchronizedFiles");
            DropTable("dbo.SourceFolders");
            DropTable("dbo.Logins");
            DropTable("dbo.Folders");
            DropTable("dbo.DocumentTypes");
        }
    }
}
