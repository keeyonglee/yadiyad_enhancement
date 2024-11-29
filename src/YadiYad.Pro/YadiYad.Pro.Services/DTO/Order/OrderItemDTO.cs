using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Service;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public string ItemName { get; set; }
        public int ProductTypeId { get; set; }
        public int RefId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int? InvoiceId { get; set; }
        public ConsultationInvitationDTO ConsultationInvitation { get; set; }
        public ServiceApplicationDTO ServiceApplication { get; set; }

        //for rematch
        public int Status { get; set; }
        public int? OffsetProOrderItemId { get; set; }

        /// <summary>
        /// for order offset use only
        /// </summary>
        public string EngagementCode { get; set; }
    }
}
