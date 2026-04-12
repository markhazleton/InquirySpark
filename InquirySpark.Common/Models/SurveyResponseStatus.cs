namespace InquirySpark.Common.Models;

/// <summary>
/// Maps to the lu_SurveyResponseStatus lookup table.
/// Values are determined from seed data in data/sqlite/InquirySpark.db.
/// </summary>
public enum SurveyResponseStatus
{
    /// <summary>
    /// Survey response is assigned and in progress (StatusId = 1).
    /// </summary>
    Assigned = 1,

    /// <summary>
    /// Additional information is required before proceeding (StatusId = 2).
    /// </summary>
    AdditionalInformationRequired = 2,

    /// <summary>
    /// Response is under supervisor review (StatusId = 3).
    /// </summary>
    SupervisorReview = 3,

    /// <summary>
    /// Response is under system review (StatusId = 4).
    /// </summary>
    SystemReview = 4,

    /// <summary>
    /// Survey response has been completed (StatusId = 5).
    /// </summary>
    Completed = 5,

    /// <summary>
    /// Survey response is inactive (StatusId = 6).
    /// </summary>
    Inactive = 6
}
