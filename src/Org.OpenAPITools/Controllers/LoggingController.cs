using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Attributes;
using Org.OpenAPITools.Converters;
using Org.OpenAPITools.Models;
using Org.OpenAPITools.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Org.OpenAPITools.Controllers
{
    /// <summary>
    /// Logging controller
    /// </summary>
    [ApiController]
    public class LoggingController : ControllerBase
    {
        public ILoggingService _loggingService;
        public LogMessageModelToLogMessageConverter _converter;

        /// <summary>
        /// Constructor with reuqired DI
        /// </summary>
        /// <param name="loggingService"></param>
        public LoggingController(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            _converter = new LogMessageModelToLogMessageConverter();
        }

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="logMessageModels">A set of log messages</param>
        /// <response code="200">Logs were succesfully queued.</response>
        [HttpPost]
        [Route("/logs/")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("LogMessage")]
        public async Task<IActionResult> LogMessages([FromBody] List<LogMessageModel> logMessageModels)
        {
            var logMessages = _converter.Convert(logMessageModels);

            var result = await _loggingService.BulkInsertLogMessages(logMessages);

            if (result == logMessageModels.Count)
            {
                return Ok("Logs were sucessfully queued");
            }
            return BadRequest(result);
        }
    }
}
