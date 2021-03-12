using NovaAlert.Entities;

namespace NovaAlert.Service.TSL
{
    public class TslTask
    {
        public Client Client { get; set; }
        public UnitPhone Unit { get; set; }
        public eTslStatusType Type { get; set; }
    }
}
