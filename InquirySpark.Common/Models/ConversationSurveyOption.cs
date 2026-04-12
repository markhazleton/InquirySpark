using System.Text.Json.Serialization;

namespace InquirySpark.Common.Models;

/// <summary>
/// A survey available for selection when starting a conversation without a specific survey_id.
/// </summary>
public class ConversationSurveyOption
{
    /// <summary>
    /// Gets or sets the survey identifier.
    /// </summary>
    [JsonPropertyName("survey_id")]
    public int SurveyId { get; set; }

    /// <summary>
    /// Gets or sets the full survey name.
    /// </summary>
    [JsonPropertyName("survey_name")]
    public string SurveyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the abbreviated survey name.
    /// </summary>
    [JsonPropertyName("survey_short_name")]
    public string SurveyShortName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the survey description.
    /// </summary>
    [JsonPropertyName("survey_description")]
    public string SurveyDescription { get; set; } = string.Empty;
}
