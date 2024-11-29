using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Operation
{
    public class OperationModel : BaseNopEntityModel
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int BusinessNatureCategoryId { get; set; }
        public int CustomerId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public int VendorId { get; set; }
        public string AccountHolderName { get; set; }
        public string BankName { get; set; }
        public string Comment { get; set; }

        #region Nested Classes

        public string CreatedDateTimeText
        {
            get
            {
                return CreatedDateTime.ToShortDateString();
            }
        }

        #endregion
    }
}
