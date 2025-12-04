using InquirySpark.Common.Extension;

namespace InquirySpark.Common.Tests.Extension;

[TestClass]
public class LogExtensionsTests
{
    [TestMethod]
    public void IsSimpleType_StateUnderTest_ExpectedBehaviorTrue()
    {
        // Arrange
        string type = "test";

        // Act
        var result = LogExtensions.IsSimpleType(type.GetType());

        // Assert
        Assert.IsTrue(result);
    }
}
