using System.Text.Json;
using InquirySpark.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InquirySpark.Common.Tests.Integration;

[TestClass]
public class ConversationApiContractTests
{
    [TestMethod]
    public void ConversationStartRequest_SerializesSnakeCaseProperties()
    {
        var request = new ConversationStartRequest
        {
            AccountName = "user1",
            Password = "p@ss",
            ApplicationId = 7,
            SurveyId = 12,
            Action = "resume"
        };

        var json = JsonSerializer.Serialize(request);

        StringAssert.Contains(json, "\"account_name\"");
        StringAssert.Contains(json, "\"application_id\"");
        StringAssert.Contains(json, "\"survey_id\"");
        StringAssert.Contains(json, "\"conversation_id\"");
        StringAssert.Contains(json, "\"action\"");
    }
}
