using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.ShippingShuq.DTO
{
    public class ShipmentCarrierDTO
    {
        public int OrderId { get; set; }
        public int ShipmentId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverAddress1 { get; set; }
        public string ReceiverAddress2 { get; set; }
        public string ReceiverZip { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverState { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string SenderAddress { get; set; }
        public string SenderAddress1 { get; set; }
        public string SenderAddress2 { get; set; }
        public string SenderZip { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string Remarks { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalValue { get; set; }
        public string SenderCompanyName { get; set; }
        public string ProductName { get; set; }
        public bool RequireInsurance { get; set; }
        public int Quantity { get; set; }
        public int? DeliveryModeId { get; set; }
        public string ScheduleAt { get; set; }
        public DeliveryMode DeliveryModeEnum
        { 
            get
            {
                return DeliveryModeId != null ? (DeliveryMode)DeliveryModeId : DeliveryMode.Bike;
            }
        }

    }
}
