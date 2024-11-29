using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core
{
    public abstract partial class BaseEntityExtension : BaseEntity
    {
        public bool Deleted { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }

        [ForeignKey("CreatedById")]
        public Customer CreatedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public Customer UpdatedBy { get; set; }


        public void CreateAudit( int actorId)
        {
            CreatedById = actorId;
            CreatedOnUTC = DateTime.UtcNow;
            UpdatedById = actorId;
            UpdatedOnUTC = DateTime.UtcNow;
        }

        public void UpdateAudit(int actorId)
        {
            UpdatedById = actorId;
            UpdatedOnUTC = DateTime.UtcNow;
        }

        public void UpdateAudit(BaseEntityExtension oldModel, int actorId)
        {
            CreatedById = oldModel.CreatedById;
            CreatedOnUTC = oldModel.CreatedOnUTC;
            UpdatedById = actorId;
            UpdatedOnUTC = DateTime.UtcNow;
        }
    }

}
