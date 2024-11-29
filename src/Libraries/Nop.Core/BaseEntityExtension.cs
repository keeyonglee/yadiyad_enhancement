using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core
{
    public abstract partial class BaseEntityExtension : BaseEntity
    {
        public bool Deleted { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }

        public void CreateAudit(int actorId)
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
