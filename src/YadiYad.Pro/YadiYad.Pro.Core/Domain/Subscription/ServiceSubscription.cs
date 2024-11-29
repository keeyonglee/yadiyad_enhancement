using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Subscription
{
    public class ServiceSubscription : BaseEntityExtension
    {
        /// <summary>
        /// *date store in UTC format
        /// subscription start date which is today's date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// *date store in UTC format
        /// subscription end date which is today's date + no of subscription day.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// *date store in UTC format
        /// date indicate the date which subscription stopped
        /// e.g. job engagment is rehired aka 2nd hiring job PVI subscription will be stopped
        /// </summary>
        public DateTime? StopDate { get; set; }
        public int CustomerId { get; set; }
        public int SubscriptionTypeId { get; set; }
        
        /// <summary>
        /// different subscription type link to different table
        /// </summary>
        public int RefId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public SubscriptionType SubscriptionType
        {
            get => (SubscriptionType)SubscriptionTypeId;
            set => SubscriptionTypeId = (int)value;
        }

    }
}
