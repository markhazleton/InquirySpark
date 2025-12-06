using System;

namespace InquirySpark.Repository.Database;

public partial class VwSurveyResponseContext
{
    public int SurveyResponseId { get; set; }

    public string SurveyResponseNm { get; set; } = null!;

    public int SurveyId { get; set; }

    public int ApplicationId { get; set; }

    public int? AssignedUserId { get; set; }

    public int StatusId { get; set; }

    public string DataSource { get; set; } = null!;

    public int ModifiedId { get; set; }

    public DateTime ModifiedDt { get; set; }

    public string SurveyNm { get; set; } = null!;

    public string SurveyShortNm { get; set; } = null!;

    public string SurveyDs { get; set; } = null!;

    public string ApplicationNm { get; set; } = null!;

    public string ApplicationCd { get; set; } = null!;

    public string ApplicationShortNm { get; set; } = null!;

    public string? EMailAddress { get; set; }

    public string? FirstNm { get; set; }

    public string? LastNm { get; set; }

    public string? AccountNm { get; set; }

    public string? CommentDs { get; set; }

    public string? SupervisorAccountNm { get; set; }

    public int? QuestionId { get; set; }

    public int? QuestionAnswerId { get; set; }

    public string? QuestionNm { get; set; }

    public string? QuestionAnswerNm { get; set; }

    public int? ApplicationUserId { get; set; }
}
