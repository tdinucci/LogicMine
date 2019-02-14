using System;
using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine.Login
{
    // Here rather than using the DefaultShaftRegistrar (which is defined within this project) we use the basic 
    // Shaft registrar provided by the framework.  We do this because we don't want to include SecurityStations within 
    // Login shafts because the caller obviously won't have an access token yet.
    public class LoginShaftRegistrar : ShaftRegistrar
    {
        private readonly ITraceExporter _traceExporter;

        public LoginShaftRegistrar(ITraceExporter traceExporter)
        {
            _traceExporter = traceExporter ?? throw new ArgumentNullException(nameof(traceExporter));
        }

        public override void RegisterShafts(IMine mine)
        {
            mine
                .AddShaft(new Shaft<LoginRequest, LoginResponse>(_traceExporter, new LoginTerminal()));
        }
    }
}