using Org.OpenAPITools.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Org.OpenAPITools.Services;

/// <summary>
/// Interface for the Logging Service
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Method used to insert log messages
    /// </summary>
    /// <param name="logMessages"></param>
    /// <returns></returns>
    Task<int> BulkInsertLogMessages(List<LogMessage> logMessages);
}
