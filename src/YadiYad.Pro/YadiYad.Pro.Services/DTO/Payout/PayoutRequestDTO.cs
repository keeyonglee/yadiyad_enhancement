using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.Payout
{
    public class PayoutRequestDTO
    {
        public string InvoiceItemName
        {
            get
            {
                var text = "";
                var feeText = "";
                var stageText = "";

                switch ((ProductType)ProductTypeId)
                {
                    case ProductType.ConsultationEngagementFee:
                        feeText = "Consultatin Fee";
                        break;
                    case ProductType.JobEnagegementFee:
                        feeText = "Job Salary";
                        break;
                    case ProductType.ServiceEnagegementFee:
                        feeText = "Service Fee";
                        break;
                }

                if(string.IsNullOrWhiteSpace(JobMilestoneName) == false)
                {
                    stageText = JobMilestoneName;
                }

                if(StartDate.HasValue && EndDate.HasValue)
                {
                    stageText = $"{StartDate.Value.ToString("dd MMM yyyy")} to {EndDate.Value.ToString("dd MMM yyyy")}";
                }

                text = $"{feeText} {stageText}";

                return text;
            }
        }

        public int Id { get; set; }
        public int? JobMilestoneId { get; set; }
        public string JobMilestoneName { get; set; }
        public int? JobMilestonePhase { get; set; }

        public int RefId { get; set; }
        public int ProductTypeId { get; set; }

        public int OrderItemId { get; set; }

        public decimal Fee { get; set; }
        public decimal ServiceCharge { get; set; }
        //public decimal? OnSiteFee { get; set; }
        public decimal ServiceChargeRate { get; set; }
        public int ServiceChargeType { get; set; }

        public int PayoutTo { get; set; }
        public int Status { get; set; }
        public string StatusName
        {
            get
            {
                var statusName = (((PayoutRequestStatus?)Status)?.GetDescription()) ?? "Unknown";
                return statusName;
            }
        }

        private string _timeSheetJson { get; set; }

        public string TimeSheetJson
        {
            get
            {
                return _timeSheetJson;
            }
            set
            {
                _timeSheetJson = value;
                WorkingTimeSlots = JsonConvert.DeserializeObject<List<WorkingTimeSlot>>(_timeSheetJson);
            }
        }

        public string WorkDesc { get; set; }
        public int? OnsiteDuration { get; set; }
        public int? ProratedWorkDuration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [UIHint("Document")]
        public int? AttachmentDownloadId { get; set; }

        public int? InvoiceId { get; set; }

        public int? ServiceChargeInvoiceId { get; set; }

        public string EnteredRemark { get; set; }

        public string Remark { get; set; }

        public List<RemarksDTO> Remarks
        {
            get
            {
                var remarkDTOs = new List<RemarksDTO>();

                if (string.IsNullOrWhiteSpace(Remark) == false)
                {
                    try
                    {
                        remarkDTOs = JsonConvert.DeserializeObject<List<RemarksDTO>>(Remark);
                    }
                    catch (JsonReaderException ex)
                    {
                        remarkDTOs.Add(new RemarksDTO
                        {
                            Remark = Remark,
                            RemarkDate = CreatedOnUTC,
                            ActorName = ""
                        });
                    }
                }

                remarkDTOs.OrderByDescending(x => x.RemarkDate);
                return remarkDTOs;
            }
        }

        public string LastRemark
        {
            get
            {
                var lastRemark = "";
                var remarkDTOs = Remarks;

                if(remarkDTOs.Count > 0)
                {
                    lastRemark = remarkDTOs.OrderByDescending(x => x.RemarkDate).Select(x => x.Remark).FirstOrDefault();
                }

                return lastRemark;
            }
        }

        public List<WorkingTimeSlot> WorkingTimeSlots{ get; set; }

        public DateTime CreatedOnUTC { get; set; }

        public DateTime UpdatedOnUTC { get; set; }

        public bool IsProrated
        {
            get
            {
                var isProrated = false;

                if (StartDate.HasValue
                    && StartDate.Value.Day != 1
                    && StartDate.Value.Day != 16)
                {
                    isProrated = true;
                }
                else if (EndDate.HasValue
                    && EndDate.Value.AddDays(1).Day != 1
                    && EndDate.Value.AddDays(1).Day != 16)
                {
                    isProrated = true;
                }

                return isProrated;
            }
        }
    }
}
