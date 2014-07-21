using System.Data.Entity.Migrations;

namespace ClaimsAuth.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Infrastructure.MyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}
