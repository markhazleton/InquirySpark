#nullable enable
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using InquirySpark.Common.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InquirySpark.Common.Tests.Integration;

/// <summary>
/// Integration test scaffold for the HATEOAS Conversation API.
/// Tests are marked [Ignore] by default — enable them when a writable
/// conversation-dev.db is available (run eng/apply-conversation-migration.ps1 first).
/// </summary>
[TestClass]
public class ConversationApiTests
{
    private static WebApplicationFactory<Program>? _factory;
    private static HttpClient? _client;

    [ClassInitialize]
    public static void ClassInit(TestContext _)
    {
        var repoRoot = FindRepoRoot(AppContext.BaseDirectory);
        var devDb = Path.Combine(repoRoot, "data", "sqlite", "conversation-dev.db");
        var userDb = Path.Combine(repoRoot, "data", "sqlite", "ControlSparkUser.db");

        // Skip setup if the writable dev database does not exist
        if (!File.Exists(devDb))
        {
            Assert.Inconclusive(
                "conversation-dev.db not found. Run eng/apply-conversation-migration.ps1 to create it.");
            return;
        }

        Environment.SetEnvironmentVariable("ConnectionStrings__InquirySparkConnection",
            $"Data Source={devDb};Mode=ReadWriteCreate");
        Environment.SetEnvironmentVariable("ConnectionStrings__ControlSparkUserContextConnection",
            $"Data Source={userDb};Mode=ReadWriteCreate");

        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
        Environment.SetEnvironmentVariable("ConnectionStrings__InquirySparkConnection", null);
        Environment.SetEnvironmentVariable("ConnectionStrings__ControlSparkUserContextConnection", null);
    }

    // ── POST /api/v1/conversation/start ───────────────────────────────────────

    /// <summary>
    /// Verifies that /start with a valid application_id but no survey_id returns
    /// a survey selection list (action_type = "survey_selection").
    /// </summary>
    [TestMethod]
    [Ignore("Requires writable conversation-dev.db — run eng/apply-conversation-migration.ps1")]
    public async Task Start_NoSurveyId_ReturnsSurveyList()
    {
        var request = new ConversationStartRequest
        {
            AccountName = "testuser",
            Password = "testpassword",
            ApplicationId = 1
        };

        var response = await _client!.PostAsJsonAsync("/api/v1/conversation/start", request);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
            "Should return 200 with survey list when no survey_id is provided.");

        var envelope = await ParseEnvelopeAsync(response);
        Assert.AreEqual("survey_selection", envelope.RootElement
            .GetProperty("action").GetProperty("actionType").GetString());
    }

    /// <summary>
    /// Verifies that /start with invalid credentials returns 401.
    /// </summary>
    [TestMethod]
    [Ignore("Requires writable conversation-dev.db with migrated schema and seeded data")]
    public async Task Start_InvalidCredentials_Returns401()
    {
        var request = new ConversationStartRequest
        {
            AccountName = "nonexistentuser",
            Password = "wrongpassword",
            ApplicationId = 1
        };

        var response = await _client!.PostAsJsonAsync("/api/v1/conversation/start", request);

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode,
            "Invalid credentials must return 401 with a generic error.");
    }

    /// <summary>
    /// Verifies that /start with a valid survey_id starts the survey and returns
    /// the first question with conversation_ended = false.
    /// </summary>
    [TestMethod]
    [Ignore("Requires writable conversation-dev.db — run eng/apply-conversation-migration.ps1")]
    public async Task Start_ValidSurveyId_ReturnsFirstQuestion()
    {
        var request = new ConversationStartRequest
        {
            AccountName = "testuser",
            Password = "testpassword",
            ApplicationId = 1,
            SurveyId = 1
        };

        var response = await _client!.PostAsJsonAsync("/api/v1/conversation/start", request);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
            "Should return 200 with first question.");

        var envelope = await ParseEnvelopeAsync(response);
        var root = envelope.RootElement;

        Assert.IsFalse(root.GetProperty("conversationEnded").GetBoolean(),
            "conversationEnded must be false at the start.");
        Assert.AreEqual("question", root.GetProperty("action").GetProperty("action_type").GetString(),
            "action_type must be 'question'.");
        Assert.IsTrue(root.GetProperty("conversationId").GetString() != "00000000-0000-0000-0000-000000000000",
            "conversationId must be set to a valid GUID.");
    }

    // ── POST /api/v1/conversation/next ────────────────────────────────────────

    /// <summary>
    /// End-to-end test: start a survey, answer all questions, verify conversation_ended = true.
    /// </summary>
    [TestMethod]
    [Ignore("Requires writable conversation-dev.db — run eng/apply-conversation-migration.ps1")]
    public async Task EndToEnd_AnswerAllQuestions_ConversationEnds()
    {
        // Step 1: Start the conversation
        var startRequest = new ConversationStartRequest
        {
            AccountName = "testuser",
            Password = "testpassword",
            ApplicationId = 1,
            SurveyId = 1
        };

        var startResponse = await _client!.PostAsJsonAsync("/api/v1/conversation/start", startRequest);
        Assert.AreEqual(HttpStatusCode.OK, startResponse.StatusCode, "Start must succeed.");

        var startEnvelope = await ParseEnvelopeAsync(startResponse);
        var conversationId = startEnvelope.RootElement.GetProperty("conversationId").GetString();

        Assert.IsNotNull(conversationId, "conversationId must be returned.");

        // Step 2: Follow next_url links until conversation_ended = true
        var ended = false;
        var maxSteps = 50; // safety limit
        var step = 0;

        while (!ended && step < maxSteps)
        {
            var nextUrl = startEnvelope.RootElement.TryGetProperty("nextUrl", out var nextUrlProp)
                ? nextUrlProp.GetString()
                : null;

            if (nextUrl == null)
            {
                ended = startEnvelope.RootElement.GetProperty("conversationEnded").GetBoolean();
                break;
            }

            // Parse question_id from the next_url
            var urlParts = nextUrl.TrimStart('/').Split('/');
            var questionId = int.Parse(urlParts[^1]);

            var firstOption = startEnvelope.RootElement
                .GetProperty("action")
                .GetProperty("question")
                .GetProperty("options")
                .EnumerateArray()
                .FirstOrDefault();

            ConversationNextRequest? answerRequest = null;
            if (firstOption.ValueKind != JsonValueKind.Undefined)
            {
                answerRequest = new ConversationNextRequest
                {
                    QuestionAnswerId = firstOption.GetProperty("id").GetInt32()
                };
            }

            var nextResponse = await _client!.PostAsJsonAsync(nextUrl, answerRequest);
            Assert.AreEqual(HttpStatusCode.OK, nextResponse.StatusCode,
                $"Next step {step + 1} must succeed for question {questionId}.");

            startEnvelope = await ParseEnvelopeAsync(nextResponse);
            ended = startEnvelope.RootElement.GetProperty("conversationEnded").GetBoolean();
            step++;
        }

        Assert.IsTrue(ended, "conversation_ended must be true after answering all questions.");
        Assert.AreEqual("complete", startEnvelope.RootElement
            .GetProperty("action").GetProperty("action_type").GetString(),
            "action_type must be 'complete' at the end.");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async Task<JsonDocument> ParseEnvelopeAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content);
    }

    private static string FindRepoRoot(string startDir)
    {
        var dir = new DirectoryInfo(startDir);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "InquirySpark.sln")))
            dir = dir.Parent;
        return dir?.FullName ?? startDir;
    }
}
