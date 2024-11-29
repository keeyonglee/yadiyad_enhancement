using Newtonsoft.Json;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.Organization;

namespace YadiYad.Pro.Services.DTO.Payout
{
    public class PayoutGroupDTO
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }
        public int Status { get; set; }
        public int PayoutTo { get; set; }
        public int PayoutBatchId { get; set; }
        public string Remarks { get; set; }
        public int RequestCount { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string BankName { get; set; }
        public int PayoutGroupId { get; set; }
        public IndividualProfileDTO IndividualProfile { get; set; }
        public OrganizationProfileDTO OrganizationProfile { get; set; }
        public Customer Customer { get; set; }
        public BankAccountDTO BankAccount { get; set; }

        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
