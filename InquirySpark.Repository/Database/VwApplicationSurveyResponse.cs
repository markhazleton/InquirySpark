using System;

namespace InquirySpark.Repository.Database;

public partial class VwApplicationSurveyResponse
{
    public int SurveyResponseId { get; set; }

    public string SurveyResponseNm { get; set; } = null!;

    public string? AccountNm { get; set; }

    public int ModifiedId { get; set; }

    public int? AssignedUserId { get; set; }

    public int StatusId { get; set; }

    public string DataSource { get; set; } = null!;

    public string? SurveyShortNm { get; set; }

    public int? AnswerCount { get; set; }

    public int? QuestionCount { get; set; }

    public int? VariantCount { get; set; }

    public int? ComplianceReview { get; set; }

    public int? PercentComplete { get; set; }

    public string? FirstNm { get; set; }

    public string? LastNm { get; set; }

    public string? EMailAddress { get; set; }

    public string? SurveyNm { get; set; }

    public int SurveyId { get; set; }

    public int ApplicationId { get; set; }

    public DateTime ModifiedDt { get; set; }

    public int? DaySinceModified { get; set; }

    public string? StatusNm { get; set; }

    public int? SupervisorApplicationUserId { get; set; }

    public string? SupervisorFirstNm { get; set; }

    public string? SupervisorLastNm { get; set; }

    public string? SupervisorAccountNm { get; set; }
}
