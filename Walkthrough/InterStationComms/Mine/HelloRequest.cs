using LogicMine;

namespace InterStationComms.Mine
{
    public class HelloRequest : Request
    {
        public string Name { get; set; }

        internal enum PeriodOfDay
        {
            Morning,
            Afternoon,
            Evening,
            Night
        }

        internal PeriodOfDay Period { get; set; }
    }
}