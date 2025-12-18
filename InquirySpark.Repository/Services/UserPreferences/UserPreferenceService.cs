using InquirySpark.Repository.Database;
using InquirySpark.Repository.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InquirySpark.Repository.Services.UserPreferences;

public interface IUserPreferenceService
{
    Task<string> GetPreferenceAsync(int userId, string key);
    Task SavePreferenceAsync(int userId, string key, string value);
    Task DeletePreferenceAsync(int userId, string key);
}

public class UserPreferenceService(InquirySparkContext context, ILogger<UserPreferenceService> logger) : IUserPreferenceService
{
    private readonly InquirySparkContext _context = context;
    private readonly ILogger<UserPreferenceService> _logger = logger;

    public async Task<string> GetPreferenceAsync(int userId, string key)
    {
        try
        {
            var preference = await _context.UserPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId && p.PreferenceKey == key);

            return preference?.PreferenceValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get preference {Key} for user {UserId}", key, userId);
            return null;
        }
    }

    public async Task SavePreferenceAsync(int userId, string key, string value)
    {
        try
        {
            var preference = await _context.UserPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId && p.PreferenceKey == key);

            if (preference == null)
            {
                preference = new UserPreferenceEntity
                {
                    UserId = userId,
                    PreferenceKey = key,
                    PreferenceValue = value,
                    ModifiedDt = DateTime.UtcNow
                };
                _context.UserPreferences.Add(preference);
            }
            else
            {
                preference.PreferenceValue = value;
                preference.ModifiedDt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save preference {Key} for user {UserId}", key, userId);
            throw;
        }
    }

    public async Task DeletePreferenceAsync(int userId, string key)
    {
        try
        {
            var preference = await _context.UserPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId && p.PreferenceKey == key);

            if (preference != null)
            {
                _context.UserPreferences.Remove(preference);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete preference {Key} for user {UserId}", key, userId);
            throw;
        }
    }
}
