using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public enum IndividualTitle
    {
        [Description("Mr")]
        Mr = 1,
        [Description("Mrs")]
        Mrs,
        [Description("Ms")]
        Ms,
        [Description("Sir")]
        Sir,
        [Description("Dr")]
        Dr,
        [Description("Prof")]
        Prof
    }

    public enum Gender
    {
        [Description("Male")]
        Male = 1,
        [Description("Female")]
        Female
    }
    public enum EmploymentStatus
    {
        [Description("Employed")]
        Employed = 1,
        [Description("Retired")]
        Retired = 2,
        [Description("Unemployed")]
        Unemployed = 3,
        //[Description("Student")]
        //Student = 4
    }
    public enum AcademicQualificationType
    {
        [Description("Primary/Secondary School/'O' Level")]
        Primary = 1,
        [Description("Higher Secondary/STPM/'A' Level")]
        HigherSecondary = 2,
        [Description("Professional Certificate")]
        ProfessionalCert = 3,
        [Description("Diploma")]
        Diploma = 4,
        [Description("Advance/Higher Diploma")]
        AdvDiploma = 5,
        [Description("Bachelor's Degree")]
        BachelorsDegree = 6,
        [Description("Professional Degree")]
        ProfessionalDegree = 7,
        [Description("Master Degree")]
        MasterDegree = 8,
        [Description("Doctorate")]
        Doctorate = 9
    }

    public enum ExperienceYear
    {
        [Description("Less Than 10 years experience")]
        YearLessThan10 = 10,
        [Description("11 to 20 years experience")]
        Year11To20 = 20,
        [Description("21 to 30 years experience")]
        Year21To30 = 30
    }

    public enum LanguageSpokenWrittenProficiency
    {
        [Description("Elementary")]
        Basic = 1,
        [Description("Limited Working")]
        Intermediate = 2,
        [Description("Professional Working")]
        Advance = 3,
        [Description("None")]
        None = 4
    }

    public enum IdentityType
    {
        [Description("Identity Card")]
        IdentityCard = 1,
        [Description("Passport")]
        Passport = 2,
    }
}
