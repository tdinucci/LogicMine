using System;
using LogicMine;

namespace Sample.LogicMine.Shop.Service.Mine.Login
{
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