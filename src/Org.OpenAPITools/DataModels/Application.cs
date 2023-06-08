using System.Collections.Generic;

namespace Org.OpenAPITools.DataModels;

/// <summary>
/// Application data model class
/// </summary>
public class Application
{
    public int Id { get; set; }
    public string Name { get; set; }


    public List<LogMessage> LogMessages;
}
