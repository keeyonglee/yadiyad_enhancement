using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.DepositRequest;

namespace YadiYad.Pro.Services.DTO.DepositRequest
{
    public class DepositDetailDTO
    {
        public decimal TotalAmount { get; set; }
        public DateTime? NextDueDate { get; set; }
        public int? NextReminderCount { get; set; }
        public string NextStatus
        {
            get
            {
                var text = NextReminderCount == 0
                    ? "New"
                    : NextReminderCount == 1
                    ? "1st Reminder sent"
                    : NextReminderCount == 2
                    ? "2nd Reminder sent"
                    : NextReminderCount == 3
                    ? "3rd Reminder sent"
                    : null;

                return text;
            }
        }

    }
}
