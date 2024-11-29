using NetBarcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Utilities
{
    public class NetBarCode
    {
        public static string BarCodeGenerator(string value)
        {
            var barCode = new Barcode(value, true);
            var barCodeBase64 = barCode.GetBase64Image();
            return "data:image/png;base64," + barCodeBase64;
        }
    }
}
