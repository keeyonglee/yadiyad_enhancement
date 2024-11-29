using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.Individual
{
    public class TaxAccountDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please enter SST Registration Number")]
        public string SSTRegNo { get; set; }
    }
}
