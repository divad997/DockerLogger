using Org.OpenAPITools.DataModels;
using Org.OpenAPITools.Models;
using System;
using System.Collections.Generic;

namespace Org.OpenAPITools.Converters;

/// <summary>
/// Custom converter used for Log Message entity
/// </summary>
public class LogMessageModelToLogMessageConverter
{
    /// <summary>
    /// Converts LogMessageModel to LogMessage
    /// </summary>
    /// <param name="logMessageModels"></param>
    /// <returns></returns>
    public List<LogMessage> Convert(List<LogMessageModel> logMessageModels)
    {
        var logMessages = new List<LogMessage>();

        foreach (var logMessageModel in logMessageModels)
        {
            var messageParts = logMessageModel.Message.Split(']', 2);
            messageParts[0] = messageParts[0].TrimStart('[');
            var logLevelNotIncluded = messageParts[0] == logMessageModel.Message;

            logMessages.Add(new LogMessage
            {
                Date = DateTimeOffset.FromUnixTimeSeconds((long)logMessageModel.LogDate).UtcDateTime,
                LogLevel = logLevelNotIncluded ? LogLevel.info : (LogLevel)Enum.Parse(typeof(LogLevel), messageParts[0]),
                Message = logLevelNotIncluded ? messageParts[0] : messageParts[1],
                Application = new Application() { Name = logMessageModel.Application }
            });
        }

        return logMessages;
    }
}
