using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.Individual
{
    public class BankAccountDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int BankId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        [UIHint("Document")]
        public int BankStatementDownloadId { get; set; }
        public string Comment { get; set; }
        public bool? IsVerified { get; set; }
        public string Status { get; set; }

        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }

        public string BankName { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public Guid BankStatementDownloadGuid { get; set; }

        public int IdentityType { get; set; }
        public string IdentityTypeName { get; set; }
        public string Identity { get; set; }

        [JsonIgnore]
        public string SaltKey { get; set; }
    }
}
