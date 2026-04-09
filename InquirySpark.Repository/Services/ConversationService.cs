using InquirySpark.Common.Models;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InquirySpark.Repository.Services;

/// <summary>
/// HATEOAS-driven survey conversation service.
/// Handles authentication, conversation lifecycle, and answer persistence.
/// </summary>
public class ConversationService(InquirySparkContext context, ILogger<ConversationService> logger) : IConversationService
{
    private readonly InquirySparkContext _context = context;
    private readonly ILogger<ConversationService> _logger = logger;

    // ─── IConversationService ────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<BaseResponse<ConversationEnvelope>> StartConversationAsync(ConversationStartRequest request)
    {
        return await DbContextHelper.ExecuteAsync<ConversationEnvelope>(async () =>
        {
            // T036 — Validate application
            if (request.ApplicationId == 0)
                ThrowDomain(400, "application_id is required.");

            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.ApplicationId == request.ApplicationId);
            if (application == null)
                ThrowDomain(400, "Application not found.");

            // T034/T035 — Authenticate user
            var user = await AuthenticateUserAsync(request.AccountName, request.Password);
            if (user == null)
                ThrowDomain(401, "Invalid credentials.");

            // T039/T040 — Resume or restart existing conversation
            if (request.ConversationId.HasValue)
            {
                return await ResumeOrRestartConversationAsync(request, user!);
            }

            // T031 — Survey list path (no survey_id)
            if (!request.SurveyId.HasValue)
            {
                return await BuildSurveyListEnvelopeAsync(request.ApplicationId);
            }

            // T024 — New conversation with explicit survey_id
            return await StartNewConversationAsync(request, user!);
        });
    }

    /// <inheritdoc />
    public async Task<BaseResponse<ConversationEnvelope>> NextStepAsync(
        Guid conversationId,
        int questionId,
        ConversationNextRequest request)
    {
        return await DbContextHelper.ExecuteAsync<ConversationEnvelope>(async () =>
        {
            var surveyResponse = await _context.SurveyResponses
                .FirstOrDefaultAsync(r => r.ConversationId == conversationId);

            if (surveyResponse == null)
                ThrowDomain(404, "Conversation not found.");

            var orderedQuestions = await GetOrderedQuestionsAsync(surveyResponse!.SurveyId);
            if (orderedQuestions.Count == 0)
                ThrowDomain(400, "Survey has no questions.");

            var currentPairIdx = orderedQuestions.FindIndex(p => p.Member.QuestionId == questionId);
            if (currentPairIdx < 0)
                ThrowDomain(400, "Question is not part of this survey.");

            var currentPair = orderedQuestions[currentPairIdx];

            // T027 — Read mode: null body or empty body returns current question without modification
            var isReadMode = request == null
                || (request.QuestionAnswerId == null && string.IsNullOrEmpty(request.UserInput));
            if (isReadMode)
            {
                return BuildEnvelope(conversationId, currentPair.Question,
                    currentPair.Member.QuestionGroupId, currentPairIdx, orderedQuestions.Count);
            }

            var question = currentPair.Question;

            // T047 — Validate answer type
            var hasOptions = question.QuestionAnswers.Any(a => a.ActiveFl);
            if (hasOptions && !question.CommentFl && request!.QuestionAnswerId == null)
                ThrowDomain(400, "question_answer_id is required for this question.");

            // T041 — Idempotent upsert: overwrite existing answer and purge all subsequent answers
            var existingAnswers = await _context.SurveyResponseAnswers
                .Where(a => a.SurveyResponseId == surveyResponse!.SurveyResponseId)
                .OrderBy(a => a.SequenceNumber)
                .ToListAsync();

            var currentAnswerIdx = existingAnswers.FindIndex(a => a.QuestionId == questionId);

            if (currentAnswerIdx >= 0)
            {
                var subsequentAnswers = existingAnswers.Skip(currentAnswerIdx + 1).ToList();
                _context.SurveyResponseAnswers.RemoveRange(subsequentAnswers);
                var existingAnswer = existingAnswers[currentAnswerIdx];
                existingAnswer.QuestionAnswerId = request!.QuestionAnswerId ?? 0;
                existingAnswer.AnswerComment = request.UserInput;
                existingAnswer.ModifiedDt = DateTime.UtcNow;
            }
            else
            {
                var newAnswer = new SurveyResponseAnswer
                {
                    SurveyResponseId = surveyResponse!.SurveyResponseId,
                    QuestionId = questionId,
                    QuestionAnswerId = request!.QuestionAnswerId ?? 0,
                    AnswerComment = request.UserInput,
                    AnswerType = request.QuestionAnswerId.HasValue ? "option" : "text",
                    SequenceNumber = existingAnswers.Count + 1,
                    ModifiedId = surveyResponse.ModifiedId,
                    ModifiedDt = DateTime.UtcNow
                };
                _context.SurveyResponseAnswers.Add(newAnswer);
            }

            await _context.SaveChangesAsync();

            var nextIdx = currentPairIdx + 1;

            if (nextIdx >= orderedQuestions.Count)
            {
                // T026 — Completion path
                surveyResponse!.StatusId = (int)SurveyResponseStatus.Completed;
                surveyResponse.ModifiedDt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var completionMessage = await GetSurveyCompletionMessageAsync(surveyResponse.SurveyId);
                return BuildCompletionEnvelope(conversationId, completionMessage);
            }

            var nextPair = orderedQuestions[nextIdx];
            return BuildEnvelope(conversationId, nextPair.Question,
                nextPair.Member.QuestionGroupId, nextIdx, orderedQuestions.Count);
        });
    }

    // ─── Private: Conversation Paths ─────────────────────────────────────────

    private async Task<ConversationEnvelope> ResumeOrRestartConversationAsync(
        ConversationStartRequest request, ApplicationUser user)
    {
        var existingResponse = await _context.SurveyResponses
            .FirstOrDefaultAsync(r => r.ConversationId == request.ConversationId!.Value);

        if (existingResponse == null)
            ThrowDomain(404, "Conversation not found.");

        if (existingResponse!.AssignedUserId != user.ApplicationUserId)
            ThrowDomain(400, "Conversation does not belong to the authenticated user.");

        var action = (request.Action ?? "resume").ToLowerInvariant();

        if (action == "restart")
        {
            var answersToDelete = await _context.SurveyResponseAnswers
                .Where(a => a.SurveyResponseId == existingResponse.SurveyResponseId)
                .ToListAsync();
            _context.SurveyResponseAnswers.RemoveRange(answersToDelete);
            existingResponse.StatusId = (int)SurveyResponseStatus.Assigned;
            existingResponse.ModifiedDt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        var orderedQuestions = await GetOrderedQuestionsAsync(existingResponse.SurveyId);
        if (orderedQuestions.Count == 0)
            ThrowDomain(400, "Survey has no questions.");

        var answeredIds = await _context.SurveyResponseAnswers
            .Where(a => a.SurveyResponseId == existingResponse.SurveyResponseId)
            .Select(a => a.QuestionId)
            .ToListAsync();

        var nextPairIdx = orderedQuestions.FindIndex(p => !answeredIds.Contains(p.Member.QuestionId));
        if (nextPairIdx < 0)
        {
            var completionMessage = await GetSurveyCompletionMessageAsync(existingResponse.SurveyId);
            return BuildCompletionEnvelope(existingResponse.ConversationId.GetValueOrDefault(), completionMessage);
        }

        var nextPair = orderedQuestions[nextPairIdx];
        return BuildEnvelope(existingResponse.ConversationId.GetValueOrDefault(), nextPair.Question,
            nextPair.Member.QuestionGroupId, nextPairIdx, orderedQuestions.Count);
    }

    private async Task<ConversationEnvelope> BuildSurveyListEnvelopeAsync(int applicationId)
    {
        var appSurveys = await _context.ApplicationSurveys
            .Include(a => a.Survey)
            .Where(a => a.ApplicationId == applicationId)
            .ToListAsync();

        return new ConversationEnvelope
        {
            ConversationId = Guid.Empty,
            ConversationEnded = false,
            UpdatedUtc = DateTime.UtcNow,
            Action = new ConversationAction
            {
                ActionType = "survey_selection",
                Surveys = appSurveys.Select(ConversationService_Mappers.ToConversationSurveyOption).ToList()
            },
            NextUrl = null,
            PrevUrl = null
        };
    }

    private async Task<ConversationEnvelope> StartNewConversationAsync(
        ConversationStartRequest request, ApplicationUser user)
    {
        var survey = await _context.ApplicationSurveys
            .Include(a => a.Survey)
            .Where(a => a.ApplicationId == request.ApplicationId && a.SurveyId == request.SurveyId!.Value)
            .Select(a => a.Survey)
            .FirstOrDefaultAsync();

        if (survey == null)
            ThrowDomain(404, "Survey not found for this application.");

        // T046 — Validate survey active
        var now = DateTime.UtcNow;
        if (survey!.StartDt.HasValue && survey.StartDt.Value > now)
            ThrowDomain(400, "Survey has not started yet.");
        if (survey.EndDt.HasValue && survey.EndDt.Value < now)
            ThrowDomain(400, "Survey has expired.");

        var questions = await GetOrderedQuestionsAsync(survey.SurveyId);

        // T045 — Survey must have questions
        if (questions.Count == 0)
            ThrowDomain(400, "Survey has no questions.");

        var surveyResponse = new SurveyResponse
        {
            SurveyId = survey.SurveyId,
            ApplicationId = request.ApplicationId,
            AssignedUserId = user.ApplicationUserId,
            StatusId = (int)SurveyResponseStatus.Assigned,
            DataSource = "ConversationAPI",
            ConversationId = Guid.NewGuid(),
            SurveyResponseNm = $"{user.AccountNm}_{survey.SurveyShortNm}_{DateTime.UtcNow:yyyyMMddHHmmss}",
            ModifiedId = user.ApplicationUserId,
            ModifiedDt = DateTime.UtcNow
        };

        _context.SurveyResponses.Add(surveyResponse);
        await _context.SaveChangesAsync();

        var firstPair = questions[0];
        return BuildEnvelope(surveyResponse.ConversationId.GetValueOrDefault(), firstPair.Question,
            firstPair.Member.QuestionGroupId, 0, questions.Count);
    }

    // ─── Private: Helpers ────────────────────────────────────────────────────

    /// <summary>
    /// Authenticates a user by account name and password, with lazy hash migration from plaintext.
    /// Returns null if authentication fails.
    /// </summary>
    private async Task<ApplicationUser?> AuthenticateUserAsync(string accountName, string password)
    {
        if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.AccountNm == accountName);

        if (user == null)
            return null;

        var hasher = new PasswordHasher<ApplicationUser>();

        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
                return null;

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = hasher.HashPassword(user, password);
                await _context.SaveChangesAsync();
            }
        }
        else if (!string.IsNullOrEmpty(user.Password))
        {
            if (user.Password != password)
                return null;

            // Lazy migration from plaintext to hashed
            user.PasswordHash = hasher.HashPassword(user, password);
            user.Password = null!;
            await _context.SaveChangesAsync();
        }
        else
        {
            return null;
        }

        return user;
    }

    /// <summary>
    /// Returns ordered (Member, Question) pairs for a survey, by GroupOrder then DisplayOrder.
    /// </summary>
    private async Task<List<(QuestionGroupMember Member, Question Question)>> GetOrderedQuestionsAsync(int surveyId)
    {
        var data = await _context.QuestionGroupMembers
            .Include(m => m.QuestionGroup)
            .Include(m => m.Question)
                .ThenInclude(q => q.QuestionAnswers)
            .Where(m => m.QuestionGroup.SurveyId == surveyId)
            .OrderBy(m => m.QuestionGroup.GroupOrder)
            .ThenBy(m => m.DisplayOrder)
            .ToListAsync();

        return data.Select(m => (m, m.Question)).ToList();
    }

    /// <summary>
    /// Builds a question-step ConversationEnvelope with HATEOAS links.
    /// </summary>
    private static ConversationEnvelope BuildEnvelope(
        Guid conversationId,
        Question question,
        int questionGroupId,
        int currentIndex,
        int totalQuestions)
    {
        var nextUrl = currentIndex + 1 < totalQuestions
            ? $"/api/v1/conversation/next/{conversationId}/{question.QuestionId}"
            : null;
        var prevUrl = currentIndex > 0
            ? $"/api/v1/conversation/next/{conversationId}/{question.QuestionId}"
            : null;

        return new ConversationEnvelope
        {
            ConversationId = conversationId,
            ConversationEnded = false,
            UpdatedUtc = DateTime.UtcNow,
            Action = new ConversationAction
            {
                ActionType = "question",
                Question = ConversationService_Mappers.ToConversationQuestion(question, questionGroupId)
            },
            NextUrl = nextUrl,
            PrevUrl = prevUrl
        };
    }

    /// <summary>
    /// Builds a survey-completion ConversationEnvelope.
    /// </summary>
    private static ConversationEnvelope BuildCompletionEnvelope(Guid conversationId, string completionMessage)
    {
        return new ConversationEnvelope
        {
            ConversationId = conversationId,
            ConversationEnded = true,
            UpdatedUtc = DateTime.UtcNow,
            Action = new ConversationAction
            {
                ActionType = "complete",
                CompletionMessage = completionMessage
            },
            NextUrl = null,
            PrevUrl = null
        };
    }

    private async Task<string> GetSurveyCompletionMessageAsync(int surveyId)
    {
        var message = await _context.Surveys
            .Where(s => s.SurveyId == surveyId)
            .Select(s => s.CompletionMessage)
            .FirstOrDefaultAsync();
        return message ?? "Thank you for completing the survey.";
    }

    /// <summary>
    /// Throws a <see cref="ConversationApiException"/> to signal a domain error with a specific HTTP status code.
    /// Caught by <see cref="DbContextHelper.ExecuteAsync{T}"/> and surfaced as a prefixed error response.
    /// </summary>
    private static void ThrowDomain(int statusCode, string message)
        => throw new ConversationApiException(statusCode, message);
}
