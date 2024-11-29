using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.DepositRequest;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.JobSeeker;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.DTO.Questionnaire;
using YadiYad.Pro.Services.DTO.Service;

namespace YadiYad.Pro.Services.DTO.Service
{
    public class ServiceApplicationDepositPayoutDetailDTO
    {
        public DepositDetailDTO Deposit { get; set; }
        public PayoutDetailDTO Payout { get; set; }
        public CalculatedFeeDTO EngagementFee { get; set; }
    }
}