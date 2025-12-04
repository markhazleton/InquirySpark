using InquirySpark.Common.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace InquirySpark.Repository.Services
{
    public static class DbContextHelper
    {
        public static async Task<BaseResponse<T>> ExecuteAsync<T>(Func<Task<T?>> dbOperation)
        {
            try
            {
                var result = await dbOperation();
                return new BaseResponse<T>(result);
            }
            catch (SqliteException ex)
            {
                return new BaseResponse<T>(BuildSqliteErrors(ex));
            }
            catch (DbUpdateException ex) // Specific exception for EF Core
            {
                return new BaseResponse<T>(["Database update error occurred.", ex.Message]);

            }
            catch (DbException ex) // General database exception
            {
                return new BaseResponse<T>(["Database error occurred.", ex.Message]);
            }
            catch (Exception ex) // General catch-all for other exceptions
            {
                return new BaseResponse<T>(["An unexpected error occurred.", ex.Message]);
            }
        }
        public static async Task<BaseResponseCollection<T>> ExecuteCollectionAsync<T>(Func<Task<List<T>>> dbOperation)
        {
            try
            {
                var result = await dbOperation();
                return new BaseResponseCollection<T>(result);
            }
            catch (SqliteException ex)
            {
                return new BaseResponseCollection<T>(BuildSqliteErrors(ex));
            }
            catch (DbUpdateException ex) // Specific exception for EF Core
            {
                return new BaseResponseCollection<T>(["Database update error occurred.", ex.Message]);

            }
            catch (DbException ex) // General database exception
            {
                return new BaseResponseCollection<T>(["Database error occurred.", ex.Message]);
            }
            catch (Exception ex) // General catch-all for other exceptions
            {
                return new BaseResponseCollection<T>(["An unexpected error occurred.", ex.Message]);
            }
        }

        private static string[] BuildSqliteErrors(SqliteException exception)
        {
            List<string> errors =
            [
                $"SQLite error {exception.SqliteErrorCode} ({exception.SqliteExtendedErrorCode}): {exception.Message}"
            ];

            switch (exception.SqliteErrorCode)
            {
                case SqliteReadOnly:
                    errors.Add("The database is mounted read-only. Ensure the asset from docs/copilot/session-2025-12-04/sqlite-data-assets.md is copied locally and Mode=ReadOnly is intentional.");
                    break;
                case SqliteCantOpen:
                    errors.Add("Unable to open the SQLite file. Verify the path in appsettings matches data/sqlite and that the file exists.");
                    break;
                case SqliteNotADatabase:
                    errors.Add("The target file is not a valid SQLite database—compare hashes with sqlite-data-assets.md.");
                    break;
            }

            return errors.ToArray();
        }

        private const int SqliteReadOnly = 8;
        private const int SqliteCantOpen = 14;
        private const int SqliteNotADatabase = 26;
    }
}
