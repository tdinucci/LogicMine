using System;
using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine.SalesSummary
{
    // Define how our custom shaft is registered
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
            // GetBasicShaft(...) will give us a shaft with a SecurityStation at the top 
            mine.AddShaft(GetBasicShaft(new SalesSummaryTerminal(_dbConnectionString)));
        }
    }
}