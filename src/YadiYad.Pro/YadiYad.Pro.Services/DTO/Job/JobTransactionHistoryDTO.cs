using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.JobSeeker;
using YadiYad.Pro.Services.DTO.Questionnaire;
using YadiYad.Pro.Services.DTO.Service;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class JobTransactionHistoryDTO
    {
        public int Id { get; set; }
        public int JobProfileId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public int Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionReference { get; set; }
    }
}