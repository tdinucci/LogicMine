using System.IO;
using LogicMine.Routing;
using LogicMine.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Database2
{
    public class Startup
    {
        private static readonly string DbFilename = Path.Combine(Path.GetTempPath(), "lm-db-walkthrough.db");

        public void ConfigureServices(IServiceCollection services)
        {
            var requestRouter = new IntelligentJsonRequestRouter(GetType().Assembly, services);

            services
                .AddSingleton(services)
                .AddSingleton<IRequestRouter<JObject>>(requestRouter)
                .AddSingleton(new DbConnectionString(CreateDb()))
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }

        private string CreateDb()
        {
            if (File.Exists(DbFilename))
                File.Delete(DbFilename);

            var connectionString = $"Data Source={DbFilename}";
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                var createManufacturerTableStatement = @"
CREATE TABLE Manufacturer
(
    Id      INTEGER        NOT NULL    PRIMARY KEY,
    Name    NVARCHAR(50)   NOT NULL
);";

                var createCarTableStatement = @"
CREATE TABLE Car 
(
    Id              INTEGER        NOT NULL  PRIMARY KEY,
    ManufacturerId  NVARCHAR(50)   NOT NULL,
    Model           NVARCHAR(50)   NOT NULL,
    EngineSize      DECIMAL(13,2)  NOT NULL,
    Year            INTEGER        NOT NULL,
    FOREIGN KEY(ManufacturerId) REFERENCES Manufacturer(Id)
);";

                using (var cmd = new SqliteCommand(createManufacturerTableStatement, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqliteCommand(createCarTableStatement, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            return connectionString;
        }
    }
}