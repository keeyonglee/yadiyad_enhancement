using System;
using System.Collections.Generic;
using System.Linq;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Services.Services.Common
{
    public interface IEngagementService
    {
        EngagementType EngagementType { get; }
        EngagementPartyTypeInfo EngagementPartyTypeInfo { get; }
        EngagementPartyInfo GetEngagingParties(int engagementId);
        bool Cancel(int engagementId, EngagementParty cancellingParty, int actorId);
        void UpdateCancel(int engagementId, DateTime submissionTime, string userRemarks, int reasonId, int? attachmentId, EngagementParty cancellationParty);
        IQueryable<EngagementPartyInfo> QueryableEngagementParties();
        DateTime? GetStartDate(int engagementId);
    }

}