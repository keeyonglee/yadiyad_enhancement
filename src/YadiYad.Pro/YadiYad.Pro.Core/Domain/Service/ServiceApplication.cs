using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Service
{
    public class ServiceApplication : BaseEntityExtension
    {
        public int CustomerId { get; set; }
        public int ServiceProfileId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Duration { get; set; }
        public string Location { get; set; }
        public string ZipPostalCode { get; set; }
        public int CityId { get; set; }
        public int? Required { get; set; }
        public bool RequesterIsRead { get; set; }
        public bool ProviderIsRead { get; set; }
        public bool IsEscrow { get; set; }
        public int Status { get; set; }
        public decimal ServiceFeeAmount { get; set; }
        public decimal CommissionFeeAmount { get; set; }
        public int ServiceProfileServiceTypeId { get; set; }
        public int ServiceProfileServiceModelId { get; set; }
        public decimal ServiceProfileServiceFee { get; set; }
        public decimal ServiceProfileOnsiteFee { get; set; }
        public string CancellationRemarks { get; set; }
        public int? CancellationReasonId { get; set; }
        public int? CancellationDownloadId { get; set; }
        public DateTime? CancellationDateTime { get; set; }
        public bool CancelRehire { get; set; }
        public bool HasRehired { get; set; }
        public int RehiredServiceApplicationId { get; set; }
        public DateTime HiredTime { get; set; }
        public bool HasCancelledTwice { get; set; }

        [ForeignKey("ServiceProfileId")]
        public ServiceProfile ServiceProfile { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("CreatedById")]
        public Customer CreatedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public Customer UpdatedBy { get; set; }
        [ForeignKey("CancellationReasonId")]
        public Reason Reason { get; set; }
    }
}
