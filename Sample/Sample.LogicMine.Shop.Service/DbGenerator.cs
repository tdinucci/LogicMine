using System.IO;
using Microsoft.Data.Sqlite;

namespace Sample.LogicMine.Shop.Service
{
    /// <summary>
    /// This is used to generate an Sqlite database for the sample service to work with.
    /// Sqlite was chosen simply because you don't need to install a DB server.
    ///
    /// Each time this generator runs the database that was generated the last time it ran is deleted,
    /// meaning each time this sample service is run you start off with no data
    /// </summary>
    public class DbGenerator
    {
        private readonly string _filename;

        public DbGenerator(string filename)
        {
            _filename = filename;
        }

        public string CreateDb()
        {
            DeleteDb();

            var connectionString = $"Data Source={_filename}";
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                foreach (var statement in GetCreateDbStatements())
                {
                    using (var cmd = new SqliteCommand(statement, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return connectionString;
        }

        private string[] GetCreateDbStatements()
        {
            return new[]
            {
                GetCreateCustomerTableStatement(),
                GetCreateProductTableStatement(),
                GetCreatePurchaseTableStatement()
            };
        }

        private string GetCreateCustomerTableStatement()
        {
            return @"
CREATE TABLE Customer 
(
    Id          INTEGER        NOT NULL  PRIMARY KEY,
    Forename    NVARCHAR(50)   NOT NULL,
    Surname     NVARCHAR(50)   NOT NULL,
    Email       NVARCHAR(255)  NOT NULL,
    CreatedAt   VARCHAR(25)    NOT NULL
);";
        }

        private string GetCreateProductTableStatement()
        {
            return @"
CREATE TABLE Product 
(
    Id          INTEGER        NOT NULL  PRIMARY KEY,
    Name        NVARCHAR(50)   NOT NULL,
    Price       DECIMAL(13,2)  NOT NULL
);";
        }

        private string GetCreatePurchaseTableStatement()
        {
            return @"
CREATE TABLE Purchase 
(
    Id              INTEGER        NOT NULL  PRIMARY KEY,
    CustomerId      INTEGER        NOT NULL  REFERENCES Customer(Id),
    ProductId       INTEGER        NOT NULL  REFERENCES Product(Id),
    Quantity        INTEGER        NOT NULL,
    UnitPrice       DECIMAL(13,2)  NOT NULL,
    UnitDiscount    DECIMAL(13,2)  NOT NULL,
    CreatedAt       VARCHAR(25)    NOT NULL
);";
        }

        public void DeleteDb()
        {
            if (File.Exists(_filename))
                File.Delete(_filename);
        }
    }
}