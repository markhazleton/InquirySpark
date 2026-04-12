using InquirySpark.Common.Models;
using InquirySpark.Repository.Database;

namespace InquirySpark.Repository.Services;

/// <summary>
/// Static mapper class for converting database entities to Conversation API DTOs.
/// </summary>
public static class ConversationService_Mappers
{
    /// <summary>
    /// Maps a <see cref="Question"/> and its answers to a <see cref="ConversationQuestion"/> DTO.
    /// </summary>
    /// <param name="question">The question entity.</param>
    /// <param name="questionGroupId">The question group identifier.</param>
    /// <returns>A conversation question DTO.</returns>
    public static ConversationQuestion ToConversationQuestion(Question question, int questionGroupId)
    {
        return new ConversationQuestion
        {
            QuestionId = question.QuestionId,
            QuestionGroupId = questionGroupId,
            Text = question.QuestionDs,
            AllowFreeText = question.CommentFl,
            Options = question.QuestionAnswers
                .Where(a => a.ActiveFl)
                .OrderBy(a => a.QuestionAnswerSort)
                .Select(ToConversationAnswerOption)
                .ToList()
        };
    }

    /// <summary>
    /// Maps a <see cref="QuestionAnswer"/> to a <see cref="ConversationAnswerOption"/> DTO.
    /// </summary>
    /// <param name="answer">The question answer entity.</param>
    /// <returns>A conversation answer option DTO.</returns>
    public static ConversationAnswerOption ToConversationAnswerOption(QuestionAnswer answer)
    {
        return new ConversationAnswerOption
        {
            Id = answer.QuestionAnswerId,
            Text = answer.QuestionAnswerNm,
            Sort = answer.QuestionAnswerSort
        };
    }

    /// <summary>
    /// Maps an <see cref="ApplicationSurvey"/> with its <see cref="Survey"/> to a <see cref="ConversationSurveyOption"/> DTO.
    /// </summary>
    /// <param name="appSurvey">The application survey entity with Survey navigation loaded.</param>
    /// <returns>A conversation survey option DTO.</returns>
    public static ConversationSurveyOption ToConversationSurveyOption(ApplicationSurvey appSurvey)
    {
        return new ConversationSurveyOption
        {
            SurveyId = appSurvey.Survey.SurveyId,
            SurveyName = appSurvey.Survey.SurveyNm,
            SurveyShortName = appSurvey.Survey.SurveyShortNm,
            SurveyDescription = appSurvey.Survey.SurveyDs
        };
    }

    /// <summary>
    /// Builds a <see cref="ConversationEnvelope"/> for a question step.
    /// </summary>
    /// <param name="conversationId">The conversation identifier.</param>
    /// <param name="question">The current question entity.</param>
    /// <param name="questionGroupId">The question group identifier.</param>
    /// <param name="currentIndex">Zero-based index of the current question in the ordered list.</param>
    /// <param name="totalQuestions">Total number of questions in the survey.</param>
    /// <param name="baseUrl">The base URL for constructing HATEOAS links (e.g. https://host/api/v1/conversation).</param>
    /// <returns>A conversation envelope DTO.</returns>
    public static ConversationEnvelope ToConversationEnvelope(
        Guid conversationId,
        Question question,
        int questionGroupId,
        int currentIndex,
        int totalQuestions,
        string baseUrl)
    {
        var nextQuestionId = currentIndex + 1 < totalQuestions ? (int?)null : null; // determined by caller
        var hasPrev = currentIndex > 0;

        return new ConversationEnvelope
        {
            ConversationId = conversationId,
            ConversationEnded = false,
            UpdatedUtc = DateTime.UtcNow,
            Action = new ConversationAction
            {
                ActionType = "question",
                Question = ToConversationQuestion(question, questionGroupId)
            },
            NextUrl = $"{baseUrl}/next/{conversationId}/{question.QuestionId}",
            PrevUrl = hasPrev ? $"{baseUrl}/next/{conversationId}/{question.QuestionId}" : null
        };
    }
}
