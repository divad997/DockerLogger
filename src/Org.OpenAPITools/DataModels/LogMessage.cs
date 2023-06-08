using System;

namespace Org.OpenAPITools.DataModels;

/// <summary>
/// Log Message data model class, containing log information
/// </summary>
public class LogMessage
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public DateTime Date { get; set; }
    public string Message { get; set; }
    public LogLevel LogLevel { get; set; }

    public Application Application { get; set; }
}

/// <summary>
/// Custom LogLevel enum 
/// </summary>
public enum LogLevel
{
    trace,
    debug,
    info,
    warn,
    error
}
