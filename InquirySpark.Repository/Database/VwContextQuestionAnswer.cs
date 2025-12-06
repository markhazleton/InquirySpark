using System;

namespace InquirySpark.Repository.Database;

public partial class VwContextQuestionAnswer
{
    public int SurveyResponseId { get; set; }

    public int StatusId { get; set; }

    public int SequenceNumber { get; set; }

    public int SurveyAnswerId { get; set; }

    public int QuestionId { get; set; }

    public int QuestionAnswerId { get; set; }

    public string QuestionNm { get; set; } = null!;

    public string QuestionAnswerNm { get; set; } = null!;

    public int SurveyId { get; set; }

    public int ApplicationId { get; set; }

    public int? AssignedUserId { get; set; }

    public int? ApplicationUserId { get; set; }

    public string AnswerType { get; set; } = null!;

    public string? AnswerComment { get; set; }

    public string SurveyTypeShortNm { get; set; } = null!;

    public string SurveyTypeNm { get; set; } = null!;
}
