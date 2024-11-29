using System.ComponentModel;

namespace Nop.Core.Domain.Catalog
{
    public enum ShuqBusinessNatureEnum
    {
        None = 0,
        
        [Description("Shuq Eats")]
        Eats = 10,

        [Description("Shuq Mart")]
        Mart = 20
    }
}