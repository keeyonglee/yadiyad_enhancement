using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.YadiyadReporting.DTO
{
    public class ReportingContactsOrganizationDTO
    {
        public string Name { get; set; }
        public string BusinessSegment { get; set; }
        public string CompanyRegistrationNo { get; set; }
        public DateTime EstablishedDate { get; set; }
        public string Address { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPosition { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonContact { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime OrganizationCreatedOnUtc { get; set; }
        public string IndividualEmail { get; set; }
        public bool IndividualEmailVerified { get; set; }
        public string IndividualFullName { get; set; }
        public string IndividualContactNumber { get; set; }
        public DateTime IndividualDOB { get; set; }
        public string IndividualUsername { get; set; }
        public DateTime LatestServiceProfileCreatedOnUTC { get; set; }
        public int ServiceProfileCount { get; set; }
        public DateTime JobCVCreatedOnUTC { get; set; }
        public string EstablishedDateText
        {
            get
            {
                return EstablishedDate.ToShortDateString();
            }
        }
        public string CreatedOnUtcText
        {
            get
            {
                return CreatedOnUtc.ToShortDateString();
            }
        }
        public string OrganizationCreatedOnUtcText
        {
            get
            {
                return OrganizationCreatedOnUtc.ToShortDateString();
            }
        }
        public string IndividualDOBText
        {
            get
            {
                return IndividualDOB.ToShortDateString();
            }
        }

        public string LatestServiceProfileCreatedOnUTCText
        {
            get
            {
                var dateText = LatestServiceProfileCreatedOnUTC.ToShortDateString();
                if (dateText == "1/1/0001")
                {
                    return "-";
                }
                else
                {
                    return dateText;

                }
            }
        }
        public string JobCVCreatedOnUTCText
        {
            get
            {
                var dateText = JobCVCreatedOnUTC.ToShortDateString();
                if (dateText == "1/1/0001")
                {
                    return "-";
                }
                else
                {
                    return dateText;

                }
            }
        }
        public string IndividualEmailVerifiedText
        {
            get
            {
                if (IndividualEmailVerified == true)
                {
                    return "Yes";
                }
                else
                {
                    return "No";
                }
            }
        }
    }
}
