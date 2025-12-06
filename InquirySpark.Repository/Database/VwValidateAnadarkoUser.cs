using System;

namespace InquirySpark.Repository.Database;

public partial class VwValidateAnadarkoUser
{
    public string RowAction { get; set; } = null!;

    public int ApplicationId { get; set; }

    public string ApplicationNm { get; set; } = null!;

    public int SurveyId { get; set; }

    public string SurveyNm { get; set; } = null!;

    public int ApplicationUserId { get; set; }

    public string? SurveyResponseNm { get; set; }

    public int? SurveyResponseId { get; set; }

    public int EmployeeApplicationUserId { get; set; }

    public string AccountNm { get; set; } = null!;

    public int? SupervisorApplicationUserId { get; set; }

    public int ApplicationUserRoleId { get; set; }

    public string? DataSource { get; set; }

    public int? StatusId { get; set; }

    public int RoleId { get; set; }
}
