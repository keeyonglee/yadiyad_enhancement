using AutoMapper;
using Newtonsoft.Json;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Order;

namespace YadiYad.Pro.Services.DTO.Service
{
    public class ServiceApplicationDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerContactNo { get; set; }
        public string BuyerEmail { get; set; }
        public string SellerName { get; set; }
        public string SellerContactNo { get; set; }
        public string SellerEmail { get; set; }
        public int SellerCancelCount { get; set; }
        public int ServiceProfileId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CancellationDateTime { get; set; }
        public string CancellationEndRemarks { get; set; }
        public int DaysAbleToCancel { get; set; }
        public bool CanCancel { get; set; }
        public bool CanRefund { get; set; }
        public int? Duration { get; set; }
        public string Location { get; set; }
        public string ZipPostalCode { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int? Required { get; set; }
        public bool RequesterIsRead { get; set; }
        public bool ProviderIsRead { get; set; }
        public bool IsRehire { get; set; }
        public bool CancelRehire { get; set; }
        public bool HasRehired { get; set; }
        public int RehiredServiceApplicationId { get; set; }
        public DateTime HiredTime { get; set; }
        public bool HasCancelledTwice { get; set; }
        public bool IsEscrow { get; set; }
        public int Status { get; set; }
        public decimal ServiceFeeAmount { get; set; }
        public decimal CommissionFeeAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int ServiceProfileServiceTypeId { get; set; }
        public int ServiceProfileServiceModelId { get; set; }
        public decimal ServiceProfileServiceFee { get; set; }
        public decimal ServiceProfileOnsiteFee { get; set; }

        public string ServiceProfileServiceTypeName { get; set; }
        public string ServiceProfileServiceModelName { get; set; }
        public string StatusName { get
            {
                var status = (ServiceApplicationStatus?)Status;

                var name = "";

                if(status != null)
                {
                    name = status.GetDescription();
                }

                return name;
            }
        }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }

        public ServiceProfileDTO ServiceProfile { get; set; }
        public string Code
        {
            get{
                string format = "00000";
                string referenceNumber = "YS" + ("000000" + Id).PadRight(format.Length);

                return referenceNumber;
            } 
        }

        public bool HasPayoutRequest { get; set; }

        public bool CanTerminate
        {
            get
            {
                var canTerminate = false;

                canTerminate = Status == (int)ServiceApplicationStatus.Paid
                    && EndDate.HasValue == false
                    && DateTime.Today >= StartDate;

                return canTerminate;
            }
        }

        public bool IsEngagementEnded
        {
            get
            {
                var isEngagementEnded = false;

                isEngagementEnded =  EndDate.HasValue == true
                    && DateTime.Today > EndDate;

                return isEngagementEnded;
            }
        }

        [JsonIgnore]
        public decimal FeePerDepositExclSST
        {
            get
            {
                decimal monthlyPayment = 0;
                var ratePerDuration = this.ServiceProfileServiceFee;

                //check onsite
                if (this.ServiceProfileServiceModelId == (int)ServiceModel.Onsite || this.ServiceProfileServiceModelId == (int)ServiceModel.PartialOnsite)
                {
                    ratePerDuration += this.ServiceProfileOnsiteFee;
                }

                //check type
                if (this.ServiceProfileServiceTypeId == (int)ServiceType.Freelancing)
                {
                    //rate * required hour per week * 4 = 1 month fee
                    monthlyPayment = ratePerDuration * this.Required.Value * 4;
                }
                else if (this.ServiceProfileServiceTypeId == (int)ServiceType.PartTime)
                {
                    //rate * 4 = 1 month fee
                    monthlyPayment = ratePerDuration * this.Required.Value;
                }
                else if (this.ServiceProfileServiceTypeId == (int)ServiceType.Consultation || this.ServiceProfileServiceTypeId == (int)ServiceType.ProjectBased)
                {
                    //by session or by project
                    monthlyPayment = ratePerDuration;
                }

                return monthlyPayment;
            }
        }
    }
}
