using Nop.Core.Configuration;

namespace Nop.Core.Domain.ShippingShuq
{
    public class CourierSettings : ISettings
    {
        public int Eats { get; set; }
        public int Mart { get; set; }
    }
}