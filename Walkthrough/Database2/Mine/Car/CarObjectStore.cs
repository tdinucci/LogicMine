using System;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.Ado;
using LogicMine.DataObject.Ado.Sqlite;
using LogicMine.DataObject.Filter;
using Microsoft.Data.Sqlite;

namespace Database2.Mine.Car
{
    public class CarObjectStore : SqliteMappedObjectStore<Car, int>
    {
        private const string BaseSelectStatement = @"
SELECT
    Car.Id, Manufacturer.Name AS Make, Car.Model, Car.Year, Car.EngineSize
FROM
    Car
    JOIN Manufacturer ON Car.ManufacturerId = Manufacturer.Id";

        public CarObjectStore(string connectionString, SqliteMappedObjectDescriptor<Car, int> descriptor,
            IDbMapper<Car> mapper = null, ITransientErrorAwareExecutor transientErrorAwareExecutor = null) : base(
            connectionString, descriptor, mapper, transientErrorAwareExecutor)
        {
        }

        protected override IDbStatement<SqliteParameter> GetSelectDbStatement(int identity, string[] fields = null)
        {
            var sql = BaseSelectStatement + " WHERE Car.Id = @CarId";
            return new SqliteStatement(sql, new SqliteParameter("@CarId", identity));
        }

        protected override IDbStatement<SqliteParameter> GetSelectDbStatement(IFilter<Car> filter, int? max = null,
            int? page = null, string[] fields = null)
        {
            if (max != null || page != null)
                throw new NotSupportedException("We won't bother with this here");

            if (filter != null)
            {
                var sqlFilter = GetDbFilter(filter);
                return new SqliteStatement($"{BaseSelectStatement} {sqlFilter.WhereClause}", sqlFilter.Parameters);
            }

            return new SqliteStatement(BaseSelectStatement);
        }

        public override async Task<int> CreateAsync(Car obj)
        {
            var manufacturerId = await GetManufacturerIdAsync(obj.Make).ConfigureAwait(false);

            var sql = @"
INSERT INTO 
    Car 
    (ManufacturerId, Model, Year, EngineSize) 
VALUES 
    (@ManufacturerId, @Model, @Year, @EngineSize);

SELECT last_insert_rowid();";

            var statement = new SqliteStatement(sql,
                new SqliteParameter("@ManufacturerId", manufacturerId),
                new SqliteParameter("@Model", obj.Model),
                new SqliteParameter("@Year", obj.Year),
                new SqliteParameter("@EngineSize", obj.EngineSize));

            // SQLite stores 64 bit integers
            return Convert.ToInt32(await DbInterface.ExecuteScalarAsync(statement).ConfigureAwait(false));
        }

        private async Task<int> GetManufacturerIdAsync(string name)
        {
            var selectSql = "SELECT Id FROM Manufacturer WHERE Name = @Name";
            var selectStatement = new SqliteStatement(selectSql, new SqliteParameter("@Name", name));
            using (var rdr = await DbInterface.GetReaderAsync(selectStatement).ConfigureAwait(false))
            {
                if (rdr.Read())
                {
                    // SQLite stores 64 bit integers
                    return Convert.ToInt32(rdr["Id"]);
                }

                var inserSql = @"
INSERT INTO Manufacturer (Name) VALUES (@Name);
SELECT last_insert_rowid();";

                var statement = new SqliteStatement(inserSql, new SqliteParameter("@Name", name));

                // SQLite stores 64 bit integers
                return Convert.ToInt32(await DbInterface.ExecuteScalarAsync(statement).ConfigureAwait(false));
            }
        }
    }
}