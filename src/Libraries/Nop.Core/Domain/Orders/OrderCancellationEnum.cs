using System.ComponentModel;

namespace Nop.Core.Domain.Orders
{
   
    public enum OrderCancellationEnum
    {
        [Description("Need to change delivery address")]
        Address = 1,

        [Description("Seller is not responsive to my inquiries")]
        Inquiries = 2,

        [Description("Modify existing order")]
        Modify = 3,

        [Description("Others / Change of mind")]
        Others = 4,
    }
}
