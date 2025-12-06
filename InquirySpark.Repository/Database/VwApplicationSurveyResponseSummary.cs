using System;

namespace InquirySpark.Repository.Database;

public partial class VwApplicationSurveyResponseSummary
{
    public string SurveyResponseNm { get; set; } = null!;

    public string StatusNm { get; set; } = null!;

    public int StatusId { get; set; }

    public string DataSource { get; set; } = null!;

    public string SurveyShortNm { get; set; } = null!;

    public int? AnswerCount { get; set; }

    public int? QuestionCount { get; set; }

    public int? CommentCount { get; set; }

    public int? PendingReviewCount { get; set; }

    public int? PercentComplete { get; set; }

    public string SurveyNm { get; set; } = null!;

    public DateTime ModifiedDt { get; set; }

    public int? DaySinceModified { get; set; }

    public int? AssignedUserId { get; set; }

    public int SurveyResponseId { get; set; }

    public int SurveyId { get; set; }

    public int ApplicationId { get; set; }

    public int ModifiedId { get; set; }

    public string? FirstNm { get; set; }

    public string? LastNm { get; set; }

    public string? EMailAddress { get; set; }

    public int? ApplicationUserId { get; set; }

    public decimal? SurveyResponseScore { get; set; }

    public string ApplicationNm { get; set; } = null!;

    public string ApplicationCd { get; set; } = null!;

    public string ApplicationShortNm { get; set; } = null!;

    public string? AccountNm { get; set; }

    public string? SupervisorAccountNm { get; set; }
}
