using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using LogicMine;
using LogicMine.Routing;
using LogicMine.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Resilience
{
    public class Startup
    {
        private Timer _timer;
        private static readonly string DbFilename = Path.Combine(Path.GetTempPath(), "lm-db-walkthrough.db");

        public void ConfigureServices(IServiceCollection services)
        {
            var dbConnString = CreateDb();
            CauseChaos();

            var requestRouter = new IntelligentJsonRequestRouter(GetType().Assembly, services);
            services
                .AddSingleton(services)
                .AddSingleton<IRequestRouter<JObject>>(requestRouter)
                .AddSingleton(new DbConnectionString(dbConnString))
                .AddSingleton<ITransientErrorAwareExecutor, MyTransientErrorAwareExecutor>()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }

        private void CauseChaos()
        {
            StrongBox<int> boxedInt = new StrongBox<int>(0);
            TimerCallback tweakBadColumn = (state) =>
            {
                var box = (StrongBox<int>) state;

                if (box.Value % 2 == 0)
                    BreakDb();
                else
                    FixDb();

                box.Value++;
            };

            _timer = new Timer(tweakBadColumn, boxedInt, TimeSpan.FromMilliseconds(1500),
                TimeSpan.FromMilliseconds(1500));
        }

        private string CreateDb()
        {
            if (File.Exists(DbFilename))
                File.Delete(DbFilename);

            var connectionString = $"Data Source={DbFilename}";
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                var createCarTableStatement = @"
CREATE TABLE Car 
(
    Id          INTEGER        NOT NULL  PRIMARY KEY,
    Make        NVARCHAR(50)   NOT NULL,
    Model       NVARCHAR(50)   NOT NULL,
    EngineSize  DECIMAL(13,2)  NOT NULL,
    Year        INTEGER        NOT NULL
);";
                using (var cmd = new SqliteCommand(createCarTableStatement, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            return connectionString;
        }

        private void BreakDb()
        {
            Console.WriteLine("Broken");
            var connectionString = $"Data Source={DbFilename}";
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqliteCommand("ALTER TABLE Car RENAME TO Argh;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void FixDb()
        {
            Console.WriteLine("Fixed");
            var connectionString = $"Data Source={DbFilename}";
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqliteCommand("ALTER TABLE Argh RENAME TO Car;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}