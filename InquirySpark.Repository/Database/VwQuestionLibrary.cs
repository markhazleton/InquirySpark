namespace InquirySpark.Repository.Database;

/// <summary>
/// Represents the view model for the Question Library.
/// </summary>
public partial class VwQuestionLibrary
{
    /// <summary>
    /// Gets or sets the question ID.
    /// </summary>
    [DisplayName("Question ID")]
    public int QuestionId { get; set; }

    /// <summary>
    /// Gets or sets the short name of the question.
    /// </summary>
    [DisplayName("Question Short Name")]
    public string QuestionShortNm { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the question.
    /// </summary>
    [DisplayName("Question Name")]
    public string QuestionNm { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the question.
    /// </summary>
    [DisplayName("Question Description")]
    public string QuestionDs { get; set; } = null!;

    /// <summary>
    /// Gets or sets the sort order of the question.
    /// </summary>
    [DisplayName("Question Sort Order")]
    public int QuestionSort { get; set; }

    /// <summary>
    /// Gets or sets the review role level of the question.
    /// </summary>
    [DisplayName("Review Role Level")]
    public int ReviewRoleLevel { get; set; }

    /// <summary>
    /// Gets or sets the value of the question.
    /// </summary>
    [DisplayName("Question Value")]
    public int QuestionValue { get; set; }

    /// <summary>
    /// Gets or sets the survey type ID of the question.
    /// </summary>
    [DisplayName("Survey Type ID")]
    public int? SurveyTypeId { get; set; }

    /// <summary>
    /// Gets or sets the short name of the survey type.
    /// </summary>
    [DisplayName("Survey Type Short Name")]
    public string? SurveyTypeShortNm { get; set; }

    /// <summary>
    /// Gets or sets the name of the survey type.
    /// </summary>
    [DisplayName("Survey Type Name")]
    public string? SurveyTypeNm { get; set; }

    /// <summary>
    /// Gets or sets the question type ID of the question.
    /// </summary>
    [DisplayName("Question Type ID")]
    public int? QuestionTypeId { get; set; }

    /// <summary>
    /// Gets or sets the code of the question type.
    /// </summary>
    [DisplayName("Question Type Code")]
    public string? QuestionTypeCd { get; set; }

    /// <summary>
    /// Gets or sets the description of the question type.
    /// </summary>
    [DisplayName("Question Type Description")]
    public string? QuestionTypeDs { get; set; }

    /// <summary>
    /// Gets or sets the answer data type of the question.
    /// </summary>
    [DisplayName("Answer Data Type")]
    public string? AnswerDataType { get; set; }

    /// <summary>
    /// Gets or sets the answer count of the question.
    /// </summary>
    [DisplayName("Answer Count")]
    public int? AnswerCount { get; set; }

    /// <summary>
    /// Gets or sets the minimum score of the question.
    /// </summary>
    [DisplayName("Minimum Score")]
    public int? MinScore { get; set; }

    /// <summary>
    /// Gets or sets the maximum score of the question.
    /// </summary>
    [DisplayName("Maximum Score")]
    public int? MaxScore { get; set; }

    /// <summary>
    /// Gets or sets the survey count of the question.
    /// </summary>
    [DisplayName("Survey Count")]
    public int? SurveyCount { get; set; }

    /// <summary>
    /// Gets or sets the comment flag of the question.
    /// </summary>
    [DisplayName("Comment Flag")]
    public int? CommentFl { get; set; }

    /// <summary>
    /// Gets or sets the unit of measure ID of the question.
    /// </summary>
    [DisplayName("Unit of Measure ID")]
    public int? UnitOfMeasureId { get; set; }

    /// <summary>
    /// Gets or sets the name of the unit of measure.
    /// </summary>
    [DisplayName("Unit of Measure Name")]
    public string? UnitOfMeasureNm { get; set; }

    /// <summary>
    /// Gets or sets the description of the unit of measure.
    /// </summary>
    [DisplayName("Unit of Measure Description")]
    public string? UnitOfMeasureDs { get; set; }

    /// <summary>
    /// Gets or sets the response answer count of the question.
    /// </summary>
    [DisplayName("Response Answer Count")]
    public int? ResponseAnswerCount { get; set; }

    /// <summary>
    /// Gets or sets the keywords of the question.
    /// </summary>
    [DisplayName("Keywords")]
    public string? Keywords { get; set; }

    /// <summary>
    /// Gets or sets the file data of the question.
    /// </summary>
    [DisplayName("File Data")]
    public byte[]? FileData { get; set; }

    /// <summary>
    /// Gets or sets the modified ID of the question.
    /// </summary>
    [DisplayName("Modified ID")]
    public int ModifiedId { get; set; }

    /// <summary>
    /// Gets or sets the modified date of the question.
    /// </summary>
    [DisplayName("Modified Date")]
    public DateTime ModifiedDt { get; set; }
}
