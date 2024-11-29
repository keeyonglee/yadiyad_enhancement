using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core;

namespace YadiYad.Pro.Services.Services.Base
{
    public class BaseService
    {
        public void CreateAudit(BaseEntityExtension model, int actorId)
        {
            model.CreatedById = actorId;
            model.CreatedOnUTC = DateTime.UtcNow;
            model.UpdatedById = actorId;
            model.UpdatedOnUTC = DateTime.UtcNow;
        }

        public void UpdateAudit(BaseEntityExtension oldModel, BaseEntityExtension model, int actorId)
        {
            model.CreatedById = oldModel.CreatedById;
            model.CreatedOnUTC = oldModel.CreatedOnUTC;
            model.UpdatedById = actorId;
            model.UpdatedOnUTC = DateTime.UtcNow;
        }
    }
}
