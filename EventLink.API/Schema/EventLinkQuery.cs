using EventLink.API.Schema.Types;
using EventLink.DataAccess.Services;
using GraphQL;
using GraphQL.Types;
using System;
using IEventService = EventLink.API.Services.IEventService;
using ILogService = EventLink.API.Services.ILogService;
using IPaymentService = EventLink.API.Services.IPaymentService;
using IUserService = EventLink.API.Services.IUserService;

namespace EventLink.API.Schema
{
    public class EventLinkQuery : ObjectGraphType
    {
        public EventLinkQuery(IEventService eventService, ILogService logService,
            IPaymentService paymentService, IUserService userService)
        {
            Name = "EventLinkQuery";

            /*************************************
             * EVENT QUERIES
             *************************************/
            Field<EventType>("event",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "eventId" }),
                resolve: context =>
                {
                    try
                    {
                        var eventId = context.GetArgument<string>("eventId");
                        return eventService.GetEvent(eventId);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            Field<ListGraphType<EventType>>("searchEvents",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "query" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "filter" }),
                resolve: context =>
                {
                    try
                    {
                        var query = context.GetArgument<string>("query");
                        var filter = context.GetArgument<string>("filter");
                        return eventService.SearchEvents(query, filter);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            /*************************************
             * LOG QUERIES
             *************************************/
            Field<LogType>("log",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<EnumerationGraphType<LogDb>>> { Name = "logDb" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "logId" }),
                resolve: context =>
                {
                    try
                    {
                        var logDb = context.GetArgument<LogDb>("logDb");
                        var logId = context.GetArgument<string>("logId");
                        return logService.GetLog(logDb, logId);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            Field<ListGraphType<LogType>>(
                "logs",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<EnumerationGraphType<LogDb>>> { Name = "logDb" }),
                resolve: context =>
                {
                    try
                    {
                        var logDb = context.GetArgument<LogDb>("logDb");
                        return logService.GetLogs(logDb);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            /*************************************
             * PAYMENT QUERIES
             *************************************/
            Field<ListGraphType<PaymentType>>("userPayments",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        return paymentService.GetUserPayments(userId);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            /*************************************
             * USER QUERIES
             *************************************/
            Field<ListGraphType<UserType>>("searchUsers",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "query" }),
                resolve: context =>
                {
                    try
                    {
                        var query = context.GetArgument<string>("query");
                        return userService.SearchUsers(query);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            Field<UserType>("user",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        return userService.GetUser(userId);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            Field<UserType>("userByEmail",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "email" }),
                resolve: context =>
                {
                    try
                    {
                        var email = context.GetArgument<string>("email");
                        return userService.GetUserByEmail(email);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            Field<ListGraphType<EventType>>("favoriteEvents",
              arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
              resolve: context =>
              {
                  try
                  {
                      var userId = context.GetArgument<string>("userId");
                      return userService.GetFavoriteEvents(userId);
                  }
                  catch (Exception e)
                  {
                      context.Errors.Add(new ExecutionError(e.Message));
                      return null;
                  }
              }
          );

            Field<ListGraphType<EventType>>("participatingEvents",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        return userService.GetParticipatingEvents(userId);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            Field<ListGraphType<EventType>>("pastEvents",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        return userService.GetPastEvents(userId);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            Field<ListGraphType<UserType>>("buddies",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        return userService.GetBuddies(userId);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            Field<ListGraphType<UserType>>("participatingBuddies",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "eventId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        var eventId = context.GetArgument<string>("eventId");
                        return userService.GetParticipatingBuddies(userId, eventId);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

            Field<ListGraphType<PaymentType>>("orderHistory",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        return userService.GetOrderHistory(userId);
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message));
                        return null;
                    }
                }
            );

        }
    }
}