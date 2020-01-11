using EventLink.API.Schema;
using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using EventLink.Logging;
using GraphQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using EventService = EventLink.API.Services.EventService;
using IEventService = EventLink.API.Services.IEventService;
using ILogService = EventLink.API.Services.ILogService;
using IPaymentService = EventLink.API.Services.IPaymentService;
using IUserService = EventLink.API.Services.IUserService;
using LogService = EventLink.API.Services.LogService;
using PaymentService = EventLink.API.Services.PaymentService;
using UserService = EventLink.API.Services.UserService;

namespace EventLink.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/graphql")]
    [ApiController]
    public class GraphQLController : ControllerBase
    {
        private readonly Logger _log = Logger.Instance;

        private readonly IEventService _eventService = EventService.Instance;
        private readonly ILogService _logService = LogService.Instance;
        private readonly IPaymentService _paymentService = PaymentService.Instance;
        private readonly IUserService _userService = UserService.Instance;

        public GraphQLController()
        {

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            if (query == null)
            {
                if (SharedConstants.VerboseLoggingMode)
                {
                    _log.Log(LogDb.User, "GraphQLController", "Task", "GraphQLQuery object is null!", LogLevel.Error, false);
                }
                throw new ArgumentNullException(nameof(query));
            }

            try
            {
                var schema = new GraphQL.Types.Schema
                {
                    Query = new EventLinkQuery(_eventService, _logService, _paymentService, _userService),
                    Mutation = new EventLinkMutation(_eventService, _logService, _paymentService, _userService),
                };

                var result = await new DocumentExecuter().ExecuteAsync(x =>
                {
                    x.Schema = schema;
                    x.Query = query.Query;
                    x.Inputs = query.Variables.ToInputs();
                    x.ExposeExceptions = true; // FOR DEBUGGING!
                    x.EnableMetrics = true; // FOR DEBUGGING!
                });

                if (result.Errors?.Count > 0)
                {
                    if (SharedConstants.VerboseLoggingMode)
                    {
                        _log.Log(LogDb.User, "GraphQLController", "Task", "API request errors: " + result.Errors, LogLevel.Error, false);
                    }
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                if (SharedConstants.VerboseLoggingMode)
                {
                    _log.Log(LogDb.User, "GraphQLController", "Task", "Exception: " + e.Message + ", " + e.StackTrace, LogLevel.Error, false);
                }
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

    }
}