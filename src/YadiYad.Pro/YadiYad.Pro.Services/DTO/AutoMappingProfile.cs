using AutoMapper;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.Organization;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.DTO.JobSeeker;
using YadiYad.Pro.Core.Domain.JobSeeker;
using System.Collections.Generic;
using System.Linq;
using System;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Services.DTO.Refund;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Services.DTO.DepositRequest;

namespace YadiYad.Pro.Web.Infrastructure
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            #region Organization
            CreateMap<OrganizationProfileDTO, OrganizationProfile>();
            CreateMap<OrganizationProfile, OrganizationProfileDTO>();
            #endregion

            #region Individual
            CreateMap<IndividualProfileDTO, IndividualProfile>();
            CreateMap<IndividualProfile, IndividualProfileDTO>();
            CreateMap<BankAccountDTO, BankAccount>();
            CreateMap<BankAccount, BankAccountDTO>();
            CreateMap<BillingAddressDTO, BillingAddress>();
            CreateMap<BillingAddress, BillingAddressDTO>();
            #endregion

            #region JobSeeeker


            CreateMap<JobSeekerProfileDTO, JobSeekerProfile>();
            CreateMap<JobSeekerCategoryDTO, JobSeekerCategory>()
                .ForMember(model => model.Expertises, options => options.MapFrom(e => ToListString('|', e.ExpertiseIds)));
            CreateMap<JobSeekerAcademicQualificationDTO, JobSeekerAcademicQualification>();
            CreateMap<JobSeekerLanguageProficiencyDTO, JobSeekerLanguageProficiency>();
            CreateMap<JobSeekerLicenseCertificateDTO, JobSeekerLicenseCertificate>();
            CreateMap<JobSeekerPreferredLocationDTO, JobSeekerPreferredLocation>();
            CreateMap<JobSeekerCVDTO, JobSeekerCV>();


            CreateMap<JobSeekerProfile, JobSeekerProfileDTO>()
                .ForMember(model => model.EmploymentStatusName, options => options.MapFrom(e => ToEmploymentStatusName(e.EmploymentStatus)));
            CreateMap<JobSeekerCategory, JobSeekerCategoryDTO>()
                .ForMember(m=>m.ExpertiseIds, o=>o.MapFrom(r=> ToIntList('|', r.Expertises)));
            CreateMap<JobSeekerAcademicQualification, JobSeekerAcademicQualificationDTO>();
            CreateMap<JobSeekerLanguageProficiency, JobSeekerLanguageProficiencyDTO>();
            CreateMap<JobSeekerLicenseCertificate, JobSeekerLicenseCertificateDTO>();
            CreateMap<JobSeekerPreferredLocation, JobSeekerPreferredLocationDTO>();
            CreateMap<JobSeekerCV, JobSeekerCVDTO>();

            #endregion

            #region Job
            CreateMap<JobProfile, JobProfileDTO>();
            CreateMap<JobProfileDTO, JobProfile>();

            CreateMap<JobMilestoneDTO, JobMilestone>();
            CreateMap<JobMilestone, JobMilestoneDTO>();
            #endregion

            #region JobApplication
            CreateMap<JobApplication, JobApplicationDTO>();
            CreateMap<JobApplicationDTO, JobApplication>();
            CreateMap<JobMilestone, JobApplicationMilestone>();
            #endregion

            #region JobInvitation
            CreateMap<JobInvitation, JobInvitationDTO>();
            CreateMap<JobInvitationDTO, JobInvitation>();
            #endregion

            #region Common
            CreateMap<ExpertiseDTO, Expertise>();
            CreateMap<Expertise, ExpertiseDTO>();

            CreateMap<InterestHobbyDTO, InterestHobby>();
            CreateMap<InterestHobby, InterestHobbyDTO>();
            #endregion

            #region Service
            CreateMap<ServiceProfileDTO, ServiceProfile>();
            CreateMap<ServiceProfile, ServiceProfileDTO>();

            CreateMap<ServiceExpertiseDTO, ServiceExpertise>();

            CreateMap<ServiceApplication, ServiceApplicationDTO>();
            CreateMap<ServiceApplicationDTO, ServiceApplication>();

            CreateMap<ServiceAcademicQualificationDTO, ServiceAcademicQualification>();
            CreateMap<ServiceAcademicQualification, ServiceAcademicQualificationDTO>();

            CreateMap<ServiceLanguageProficiencyDTO, ServiceLanguageProficiency>();
            CreateMap<ServiceLanguageProficiency, ServiceLanguageProficiencyDTO>();

            CreateMap<ServiceLicenseCertificateDTO, ServiceLicenseCertificate>();
            CreateMap<ServiceLicenseCertificate, ServiceLicenseCertificateDTO>();

            #endregion

            #region Consultation
            CreateMap<ConsultationProfile, ConsultationProfileDTO>();
            CreateMap<ConsultationProfileDTO, ConsultationProfile>();

            CreateMap<ConsultationExpertiseDTO, ConsultationExpertise>();

            CreateMap<ConsultationInvitation, ConsultationInvitationDTO>();
            CreateMap<ConsultationInvitationDTO, ConsultationInvitation>();
            #endregion

            #region Order
            CreateMap<ProOrder, OrderDTO>();
            CreateMap<OrderDTO, ProOrder>();

            CreateMap<ProOrderItem, OrderItemDTO>();
            CreateMap<OrderItemDTO, ProOrderItem>();

            CreateMap<ProInvoice, InvoiceDTO>();
            CreateMap<InvoiceDTO, ProInvoice>();

            CreateMap<Charge, ChargeDTO>();
            CreateMap<ChargeDTO, Charge>();

            CreateMap<Transaction, TransactionDTO>();
            CreateMap<TransactionDTO, Transaction>();
            #endregion

            #region Payout Request

            CreateMap<PayoutRequest, PayoutRequestDTO>();
            CreateMap<PayoutRequestDTO, PayoutRequest>();

            #endregion

            #region Payout Batch

            CreateMap<PayoutBatch, PayoutBatchDTO>();
            CreateMap<PayoutBatchDTO, PayoutBatch>();

            #endregion


            #region Payout Group

            CreateMap<PayoutGroup, PayoutGroupDTO>();
            CreateMap<PayoutGroupDTO, PayoutGroup>();

            #endregion

            #region Refund
            CreateMap<RefundRequest, RefundRequestDTO>();
            CreateMap<RefundRequestDTO, RefundRequest>();
            #endregion

            #region Deposit Request
            CreateMap<DepositRequest, DepositRequestDTO>();
            CreateMap<DepositRequestDTO, DepositRequest>();
            #endregion
        }


        public static string ToListString(char seperator, List<int> values)
        {
            var strValue = '|' + string.Join(seperator, values) + '|';

            return strValue;
        }

        public static List<int> ToIntList(char seperator, string strvalues)
        {
            var values = strvalues?
                .Split(seperator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x))
                .ToList();

            return values;
        }

        public static string ToEmploymentStatusName(int employmentStatus)
        {
            var value = ((EmploymentStatus)employmentStatus).GetDescription();

            return value;
        }
    }
}
