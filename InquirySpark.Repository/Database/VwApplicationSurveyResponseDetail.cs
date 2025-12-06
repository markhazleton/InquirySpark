using System;

namespace InquirySpark.Repository.Database;

public partial class VwApplicationSurveyResponseDetail
{
    public int SurveyAnswerId { get; set; }

    public string AnswerType { get; set; } = null!;

    public double? AnswerQuantity { get; set; }

    public DateTime? AnswerDate { get; set; }

    public string? AnswerComment { get; set; }

    public string? ModifiedComment { get; set; }

    public int QuestionId { get; set; }

    public string QuestionNm { get; set; } = null!;

    public string QuestionDs { get; set; } = null!;

    public int QuestionSort { get; set; }

    public int ReviewRoleLevel { get; set; }

    public bool QuestionCommentFl { get; set; }

    public int QuestionValue { get; set; }

    public int QuestionTypeId { get; set; }

    public string QuestionTypeCd { get; set; } = null!;

    public string QuestionTypeDs { get; set; } = null!;

    public string AnswerDataType { get; set; } = null!;

    public int QuestionAnswerId { get; set; }

    public int QuestionAnswerSort { get; set; }

    public string QuestionAnswerShortNm { get; set; } = null!;

    public string QuestionAnswerNm { get; set; } = null!;

    public int QuestionAnswerValue { get; set; }

    public string QuestionAnswerDs { get; set; } = null!;

    public bool QuestionAnswerCommentFl { get; set; }

    public bool ActiveFl { get; set; }

    public int SurveyResponseId { get; set; }

    public string SurveyResponseNm { get; set; } = null!;

    public string DataSource { get; set; } = null!;

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

    public int ApplicationId { get; set; }

    public string ApplicationNm { get; set; } = null!;

    public string ApplicationCd { get; set; } = null!;

    public string ApplicationShortNm { get; set; } = null!;

    public string? ApplicationDs { get; set; }

    public int MenuOrder { get; set; }

    public int SurveyTypeId { get; set; }

    public string SurveyTypeShortNm { get; set; } = null!;

    public string SurveyTypeNm { get; set; } = null!;

    public string? SurveyTypeDs { get; set; }

    public string? SurveyTypeComment { get; set; }

    public bool MutiSequenceFl { get; set; }

    public int ApplicationTypeId { get; set; }

    public string ApplicationTypeNm { get; set; } = null!;

    public string? ApplicationTypeDs { get; set; }

    public string QuestionShortNm { get; set; } = null!;

    public string StatusNm { get; set; } = null!;

    public string StatusDs { get; set; } = null!;

    public string EmailTemplate { get; set; } = null!;

    public string EmailSubjectTemplate { get; set; } = null!;

    public string? FirstNm { get; set; }

    public string? LastNm { get; set; }

    public string? EMailAddress { get; set; }

    public string? CommentDs { get; set; }

    public string? AccountNm { get; set; }

    public string? SupervisorAccountNm { get; set; }

    public DateTime? LastLoginDt { get; set; }

    public string? LastLoginLocation { get; set; }

    public int SurveyResponseSequenceId { get; set; }

    public int SequenceNumber { get; set; }

    public string? SequenceText { get; set; }

    public int QuestionGroupMemberId { get; set; }

    public decimal QuestionWeight { get; set; }

    public int DisplayOrder { get; set; }

    public int QuestionGroupId { get; set; }

    public int GroupOrder { get; set; }

    public string QuestionGroupShortNm { get; set; } = null!;

    public string QuestionGroupNm { get; set; } = null!;

    public string? QuestionGroupDs { get; set; }

    public decimal QuestionGroupWeight { get; set; }

    public string? GroupHeader { get; set; }

    public string? GroupFooter { get; set; }

    public int? DependentQuestionGroupId { get; set; }

    public decimal? DependentMinScore { get; set; }

    public decimal? DependentMaxScore { get; set; }
}
