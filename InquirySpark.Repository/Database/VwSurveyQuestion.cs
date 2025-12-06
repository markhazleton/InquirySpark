using System;

namespace InquirySpark.Repository.Database;

public partial class VwSurveyQuestion
{
    public decimal QuestionWeight { get; set; }

    public int DisplayOrder { get; set; }

    public int QuestionId { get; set; }

    public string QuestionNm { get; set; } = null!;

    public string QuestionShortNm { get; set; } = null!;

    public string QuestionDs { get; set; } = null!;

    public int QuestionSort { get; set; }

    public int UnitOfMeasureId { get; set; }

    public int ReviewRoleLevel { get; set; }

    public int QuestionTypeId { get; set; }

    public int QuestionValue { get; set; }

    public bool CommentFl { get; set; }

    public string QuestionTypeCd { get; set; } = null!;

    public string QuestionTypeDs { get; set; } = null!;

    public string ControlName { get; set; } = null!;

    public string AnswerDataType { get; set; } = null!;

    public int SurveyId { get; set; }

    public bool UseQuestionGroupsFl { get; set; }

    public string SurveyNm { get; set; } = null!;

    public string SurveyShortNm { get; set; } = null!;

    public string SurveyDs { get; set; } = null!;

    public string CompletionMessage { get; set; } = null!;

    public string? ResponseNmtemplate { get; set; }

    public string? ReviewerAccountNm { get; set; }

    public string? AutoAssignFilter { get; set; }

    public DateTime? StartDt { get; set; }

    public DateTime? EndDt { get; set; }

    public int QuestionGroupId { get; set; }

    public int GroupOrder { get; set; }

    public string QuestionGroupShortNm { get; set; } = null!;

    public string QuestionGroupNm { get; set; } = null!;

    public string? QuestionGroupDs { get; set; }

    public decimal QuestionGroupWeight { get; set; }

    public string? GroupHeader { get; set; }

    public string? GroupFooter { get; set; }

    public int QuestionAnswerId { get; set; }

    public string QuestionAnswerShortNm { get; set; } = null!;

    public string QuestionAnswerNm { get; set; } = null!;

    public int QuestionAnswerValue { get; set; }

    public string QuestionAnswerDs { get; set; } = null!;

    public int QuestionAnswerSort { get; set; }

    public bool QuestionAnswerCommentFl { get; set; }

    public bool ActiveFl { get; set; }
}
