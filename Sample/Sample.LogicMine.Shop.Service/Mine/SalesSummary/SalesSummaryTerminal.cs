using System;
using System.Data;
using System.Threading.Tasks;
using LogicMine;
using LogicMine.DataObject.Ado.Sqlite;

namespace Sample.LogicMine.Shop.Service.Mine.SalesSummary
{
    public class SalesSummaryTerminal : Terminal<SalesSummaryRequest, SalesSummaryResponse>
    {
        private readonly string _dbConnectionString;

        public SalesSummaryTerminal(string dbConnectionString)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dbConnectionString));

            _dbConnectionString = dbConnectionString;
        }

        public override async Task AddResponseAsync(IBasket<SalesSummaryRequest, SalesSummaryResponse> basket)
        {
            var db = new SqliteInterface(_dbConnectionString);
            var statement = new SqliteStatement(@"
SELECT 
    COUNT(*) AS NumberOfSales, SUM(Quantity * UnitPrice) AS Sales, SUM(Quantity * UnitDiscount) AS Discounts 
FROM 
    Purchase");

            using (var rdr = await db.GetReaderAsync(statement).ConfigureAwait(false))
            {
                if (!await rdr.ReadAsync().ConfigureAwait(false))
                    throw new InvalidOperationException("Failed to query sales data");

                var numSales = Convert.ToInt32(GetRecordValue(rdr, "NumberOfSales"));
                var sales = Convert.ToDecimal(GetRecordValue(rdr, "Sales"));
                var discounts = Convert.ToDecimal(GetRecordValue(rdr, "Discounts"));

                basket.Response = new SalesSummaryResponse(basket.Request)
                {
                    NumberOfSales = numSales,
                    Sales = Math.Round(sales, 2),
                    Discounts = Math.Round(discounts, 2)
                };
            }
        }

        private object GetRecordValue(IDataRecord rdr, string field)
        {
            var value = rdr[field];
            return value == DBNull.Value ? null : value;
        }
    }
}