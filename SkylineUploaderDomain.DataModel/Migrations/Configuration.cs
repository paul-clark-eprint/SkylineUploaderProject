namespace SkylineUploaderDomain.DataModel.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<SkylineUploaderDomain.DataModel.UploaderDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "SkylineUploaderDomain.DataModel.UploaderContext";
        }

        protected override void Seed(SkylineUploaderDomain.DataModel.UploaderDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
