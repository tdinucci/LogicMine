using System;
using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine.SalesSummary
{
    public class SalesSummaryShaftRegistrar : DefaultShaftRegistrar
    {
        private readonly string _dbConnectionString;

        public SalesSummaryShaftRegistrar(DbConnectionString dbConnectionString, ITraceExporter traceExporter) :
            base(traceExporter)
        {
            if (dbConnectionString == null) throw new ArgumentNullException(nameof(dbConnectionString));
            if (traceExporter == null) throw new ArgumentNullException(nameof(traceExporter));

            _dbConnectionString = dbConnectionString.Value;
        }

        public override void RegisterShafts(IMine mine)
        {
            mine.AddShaft(GetBasicShaft(new SalesSummaryTerminal(_dbConnectionString)));
        }
    }
}