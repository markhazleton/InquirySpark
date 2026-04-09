namespace InquirySpark.Common.Models;

/// <summary>
/// Describes the current action the client should render in a conversation step.
/// </summary>
public class ConversationAction
{
    /// <summary>
    /// Gets or sets the action type. One of: "question", "survey_selection", "complete".
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the question to display. Non-null when ActionType is "question".
    /// </summary>
    public ConversationQuestion Question { get; set; }

    /// <summary>
    /// Gets or sets the list of available surveys. Non-null when ActionType is "survey_selection".
    /// </summary>
    public List<ConversationSurveyOption> Surveys { get; set; }

    /// <summary>
    /// Gets or sets the completion message. Non-null when ActionType is "complete".
    /// </summary>
    public string CompletionMessage { get; set; }
}
