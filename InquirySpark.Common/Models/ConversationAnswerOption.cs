namespace InquirySpark.Common.Models;

/// <summary>
/// A predefined answer option for a survey question.
/// </summary>
public class ConversationAnswerOption
{
    /// <summary>
    /// Gets or sets the answer option identifier (QuestionAnswerId).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display text for this answer option.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sort order for display.
    /// </summary>
    public int Sort { get; set; }
}
