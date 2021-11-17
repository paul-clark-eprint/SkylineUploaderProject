using System.Data.Entity;
using SkylineUploaderDomain.DataModel.Classes;


namespace SkylineUploaderDomain.DataModel
{
    public class UploaderDbContext : DbContext
    {
        //public UploaderDbContext() : base("SkylineUploader")
        public UploaderDbContext()
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<UploaderDbContext, Migrations.Configuration>());
            
        }



        public DbSet<Folder> Folders { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<Proxy> Proxy { get; set; }
        public DbSet<UserLibrary> UserLibraries { get; set; }
        public DbSet<SourceFolder> SourceFolders { get; set; }
        public DbSet<SynchronizedFile> SynchronizedFiles { get; set; }
        public DbSet<ServiceSettings> ServiceSettings { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //}

    }
}
