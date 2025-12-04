using InquirySpark.Common.Extension;
using System.ComponentModel.DataAnnotations;

namespace InquirySpark.Common.Tests.Extension;

/// <summary>
/// Enum TestEnum
/// </summary>
public enum TestEnum
{
    /// <summary>
    /// The unknown
    /// </summary>
    [Display(Name = "Unknown", Description = "Unknown")]
    Unknown = 0,

    /// <summary>
    /// The first
    /// </summary>
    [Display(Name = "1st", Description = "The First One")]
    First = 1,

    /// <summary>
    /// The second
    /// </summary>
    Second = 2
}


[TestClass]
public class EnumExtensionTests
{
    /// <summary>
    /// Defines the test method EnumGetDisplayNameExpectedBehavior.
    /// </summary>
    [TestMethod]
    public void EnumGetDisplayNameExpectedBehavior()
    {
        // Arrange
        var myTest = TestEnum.First;

        // Act
        var result = myTest.GetDisplayName();

        // Assert
        Assert.AreEqual("1st", result);
    }

    /// <summary>
    /// Defines the test method EnumGetDisplayNameMissingDisplayName.
    /// </summary>
    [TestMethod]
    public void EnumGetDisplayNameMissingDisplayName()
    {
        // Arrange
        var myTest = TestEnum.Second;

        // Act
        var result = myTest.GetDisplayName();

        // Assert
        Assert.AreEqual("Second", result);
    }

    /// <summary>
    /// Defines the test method EnumToDictionaryExpectedBehavior.
    /// </summary>
    [TestMethod]
    public void EnumToDictionaryExpectedBehavior()
    {
        // Arrange
        var myTest = TestEnum.First;

        // Act
        var result = myTest.ToDictionary();

        // Assert
        Assert.HasCount(3, result);
    }

}
