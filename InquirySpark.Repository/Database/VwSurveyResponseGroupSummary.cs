using System;

namespace InquirySpark.Repository.Database;

public partial class VwSurveyResponseGroupSummary
{
    public int SurveyResponseId { get; set; }

    public string SurveyResponseNm { get; set; } = null!;

    public int SurveyId { get; set; }

    public int ApplicationId { get; set; }

    public int? AssignedUserId { get; set; }

    public int StatusId { get; set; }

    public string DataSource { get; set; } = null!;

    public string ApplicationNm { get; set; } = null!;

    public string ApplicationCd { get; set; } = null!;

    public string ApplicationShortNm { get; set; } = null!;

    public string? StatusNm { get; set; }

    public string? SurveyNm { get; set; }

    public string? SurveyShortNm { get; set; }

    public string? FirstNm { get; set; }

    public string? LastNm { get; set; }

    public string? EMailAddress { get; set; }

    public string? AccountNm { get; set; }

    public DateTime? LastLoginDt { get; set; }

    public string? LastLoginLocation { get; set; }

    public int ModifiedId { get; set; }

    public DateTime ModifiedDt { get; set; }

    public int? AnswerCount { get; set; }

    public int? QuestionCount { get; set; }

    public int? CommentCount { get; set; }

    public int? DaySinceModified { get; set; }

    public decimal? SurveyResponseScore { get; set; }

    public decimal? SurveyResponseGroupScore { get; set; }

    public int? AverageQuestionScore { get; set; }

    public int? QuestionGroupId { get; set; }

    public int? GroupOrder { get; set; }

    public string? QuestionGroupShortNm { get; set; }

    public string? QuestionGroupNm { get; set; }

    public string? QuestionGroupDs { get; set; }

    public decimal? QuestionGroupWeight { get; set; }
}
