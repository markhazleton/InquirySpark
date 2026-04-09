namespace InquirySpark.Common.Models;

/// <summary>
/// A single question presented to the user during a conversation step.
/// </summary>
public class ConversationQuestion
{
    /// <summary>
    /// Gets or sets the question identifier.
    /// </summary>
    public int QuestionId { get; set; }

    /// <summary>
    /// Gets or sets the question group identifier.
    /// </summary>
    public int QuestionGroupId { get; set; }

    /// <summary>
    /// Gets or sets the question text displayed to the user.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of predefined answer options.
    /// </summary>
    public List<ConversationAnswerOption> Options { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the user may provide free-text input.
    /// </summary>
    public bool AllowFreeText { get; set; }
}
