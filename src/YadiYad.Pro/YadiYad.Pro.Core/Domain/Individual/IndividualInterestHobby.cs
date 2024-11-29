using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Individual
{
    public class IndividualInterestHobby : BaseEntityExtension
    {
        public int CustomerId { get; set; }
        public int InterestHobbyId { get; set; }
    }
}