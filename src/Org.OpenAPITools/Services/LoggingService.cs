using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using NpgsqlTypes;
using Org.OpenAPITools.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Org.OpenAPITools.Services;

/// <summary>
/// Logging service
/// </summary>
public class LoggingService : ILoggingService
{
    private NpgsqlConnection _connection;
    private readonly IMemoryCache _cache;
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private Dictionary<string, int> _cachedApplications;

    /// <summary>
    /// Constructor with required DI
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="cache"></param>
    public LoggingService(NpgsqlConnection connection, IMemoryCache cache)
    {
        _connection = connection;
        _cache = cache;
    }

    /// <summary>
    /// Method used for inserting LogMessages in bulk
    /// </summary>
    /// <param name="logMessages"></param>
    /// <returns></returns>
    public async Task<int> BulkInsertLogMessages(List<LogMessage> logMessages)
    {
        using (_connection)
        {
            await _connection.OpenAsync();

            try
            {
                await InsertApplicationsTransaction(logMessages);

                var result = await InsertLogMessagesTransaction(logMessages);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Method used for inserting new applications and mapping them to the cache, uses semaphore for avoiding duplicate inserts
    /// </summary>
    /// <param name="logMessages"></param>
    /// <returns></returns>
    private async Task InsertApplicationsTransaction(List<LogMessage> logMessages)
    {
        using (var transaction = await _connection.BeginTransactionAsync())
        {
            await _semaphore.WaitAsync();
            _cachedApplications = _cache.Get<Dictionary<string, int>>("applications") ?? new Dictionary<string, int>();
            try
            {
                // Filter non-cached applications
                var nonCachedApplications = logMessages.Distinct()
                                                       .Where(x => !_cachedApplications.ContainsKey(x.Application.Name))
                                                       .Select(x => x.Application.Name)
                                                       .ToList();

                // If there is a non-cached application, get or create it
                if (nonCachedApplications.Any())
                {
                    // Get non-cached applications
                    var selectCommand = new NpgsqlCommand($@"SELECT id, name FROM application WHERE name = ANY(@names)", _connection, transaction);
                    selectCommand.Parameters.AddWithValue("@names", nonCachedApplications.ToArray());

                    using (var reader = await selectCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var name = reader.GetString(1);
                            var id = reader.GetInt32(0);
                            _cachedApplications[name] = id;
                        }
                    }

                    // Build a list of new application names
                    var newApplicationNames = logMessages.Select(x => x.Application.Name)
                                                         .Except(_cachedApplications.Keys)
                                                         .ToList();

                    // Insert new applications in bulk
                    if (newApplicationNames.Any())
                    {
                        var values = string.Join(", ", newApplicationNames.Select(name => $"('{name}')"));
                        var bulkInsertApps = new NpgsqlCommand($@"
                                INSERT INTO application (name)
                                VALUES {values}
                                RETURNING id, name", _connection, transaction);

                        // Map the results to the cached applications
                        using (var reader = await bulkInsertApps.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var id = reader.GetInt32(0);
                                var name = reader.GetString(1);
                                _cachedApplications[name] = id;
                            }
                        }
                    }

                    await transaction.CommitAsync();

                    // Set cache options
                    var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30));

                    // Cache the updated Dictionary
                    _cache.Set("applications", _cachedApplications, cacheOptions);
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    /// <summary>
    /// Method used for inserting LogMessages in bulk
    /// </summary>
    /// <param name="logMessages"></param>
    /// <returns></returns>
    private async Task<int> InsertLogMessagesTransaction(List<LogMessage> logMessages)
    {
        using (var transaction = await _connection.BeginTransactionAsync())
        {
            try
            {
                _cachedApplications = _cache.Get<Dictionary<string, int>>("applications") ?? new Dictionary<string, int>();

                // Build a list of parameter values for bulk insert
                var parameterValues = logMessages.Select(logMessage =>
                    new object[] { _cachedApplications[logMessage.Application.Name], logMessage.Date, logMessage.LogLevel.ToString(), logMessage.Message }
                ).ToList();

                // Bulk insert log messages
                var bulkInsertLogs = new NpgsqlCommand($@"
                    INSERT INTO logmessage (application_id, date, log_level, message) 
                    VALUES {string.Join(", ", parameterValues.Select((_, i) => $"(@applicationId{i}, @date{i}, CAST(@logLevel{i} AS loglevel), @message{i})"))}", _connection, transaction);

                bulkInsertLogs.Parameters.AddRange(parameterValues.SelectMany((pv, i) => new NpgsqlParameter[]
                {
                    new NpgsqlParameter($"@applicationId{i}", NpgsqlDbType.Integer) { Value = pv[0] },
                    new NpgsqlParameter($"@date{i}", NpgsqlDbType.TimestampTz) { Value = pv[1] },
                    new NpgsqlParameter($"@logLevel{i}", NpgsqlDbType.Varchar) { Value = pv[2] },
                    new NpgsqlParameter($"@message{i}", NpgsqlDbType.Text) { Value = pv[3] }
                }).ToArray());

                await bulkInsertLogs.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                return logMessages.Count;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
