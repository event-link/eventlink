using EventLink.API.Schema.Types;
using EventLink.API.Schema.Types.LogTypes;
using EventLink.API.Schema.Types.UserTypes;
using EventLink.API.Services;
using EventLink.DataAccess.Models;
using GraphQL;
using GraphQL.Types;
using System;

namespace EventLink.API.Schema
{
    public class EventLinkMutation : ObjectGraphType<object>
    {
        public EventLinkMutation(IEventService eventService, ILogService logService,
            IPaymentService paymentService, IUserService userService)
        {
            Name = "EventLinkMutation";

            /*************************************
             * EVENT MUTATIONS
             *************************************/

            /*************************************
             * LOG MUTATIONS
             *************************************/
            Field<LogType>(
                "createLog",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<LogCreateInputType>> { Name = "logInput" }),
                resolve: context =>
                {
                    try
                    {
                        var logInput = context.GetArgument<LogCreateInput>("logInput");
                        var log = new Log(logInput.ParentName, logInput.FunctionName, logInput.Message, logInput.LogLevel);
                        logService.CreateLog(logInput.LogDb, log);
                        return log;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            /*************************************
             * PAYMENT MUTATIONS
             *************************************/
            Field<PaymentType>(
                "buyTicket",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "eventId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        var eventId = context.GetArgument<string>("eventId");
                        paymentService.BuyTicket(userId, eventId);
                        return null;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            /*************************************
             * USER MUTATIONS
             *************************************/
            Field<UserType>(
                "createUser",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<UserCreateInputType>> { Name = "userInput" }),
                resolve: context =>
                {
                    try
                    {
                        var userInput = context.GetArgument<UserCreateInput>("userInput");
                        var user = new User(userInput.AccountType, userInput.LoginMethod, userInput.PicUrl, userInput.FirstName, userInput.MiddleName, userInput.LastName, userInput.FullName,
                            userInput.Email, userInput.Address, userInput.Birthdate, userInput.HashedPassword, userInput.PhoneNumber, userInput.Country, userInput.ParticipatingEvents,
                            userInput.FavoriteEvents, userInput.PastEvents, userInput.Buddies, userInput.Payments, userInput.LastLoginDate, userInput.IsActive);
                        userService.CreateUser(user);
                        return user;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            Field<UserType>(
                "updateUser",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<UserUpdateInputType>> { Name = "userInput" }),
                resolve: context =>
                {
                    try
                    {
                        var userInput = context.GetArgument<UserUpdateInput>("userInput");
                        var user = new User(userInput.Id, userInput.AccountType, userInput.LoginMethod, userInput.PicUrl, userInput.FirstName,
                            userInput.MiddleName, userInput.LastName, userInput.FullName, userInput.Email, userInput.Address, userInput.Birthdate,
                            userInput.HashedPassword, userInput.PhoneNumber, userInput.Country, userInput.ParticipatingEvents,
                            userInput.FavoriteEvents, userInput.PastEvents, userInput.Buddies, userInput.Payments,
                            userInput.LastActivityDate, userInput.IsActive)
                        {
                            DbCreatedDate = userInput.DbCreatedDate,
                            DbModifiedDate = userInput.DbModifiedDate,
                            DbDeletedDate = userInput.DbDeletedDate,
                            DbReactivatedDate = userInput.DbReactivatedDate,
                            IsDeleted = userInput.IsDeleted
                        };

                        userService.UpdateUser(user);
                        return user;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            Field<StringGraphType>(
                "deactivateUser",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        userService.DeactivateUser(userId);
                        return userId;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            Field<StringGraphType>(
                "addFavoriteEvent",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "eventId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        var eventId = context.GetArgument<string>("eventId");
                        userService.AddFavoriteEvent(userId, eventId);
                        return eventId;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            Field<StringGraphType>(
                "RemoveFavoriteEvent",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "eventId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        var eventId = context.GetArgument<string>("eventId");
                        userService.RemoveFavoriteEvent(userId, eventId);
                        return eventId;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            Field<StringGraphType>(
                "addParticipatingEvent",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "eventId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        var eventId = context.GetArgument<string>("eventId");
                        userService.AddParticipatingEvent(userId, eventId);
                        return eventId;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            Field<StringGraphType>(
                "removeParticipatingEvent",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "eventId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        var eventId = context.GetArgument<string>("eventId");
                        userService.RemoveParticipatingEvent(userId, eventId);
                        return eventId;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            Field<StringGraphType>(
                "addPastEvent",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "eventId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        var eventId = context.GetArgument<string>("eventId");
                        userService.AddPastEvent(userId, eventId);
                        return eventId;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );


            Field<StringGraphType>(
                "removePastEvent",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "eventId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        var eventId = context.GetArgument<string>("eventId");
                        userService.RemovePastEvent(userId, eventId);
                        return eventId;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            Field<StringGraphType>(
                "addBuddy",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "buddyId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        var buddyId = context.GetArgument<string>("buddyId");
                        userService.AddBuddy(userId, buddyId);
                        return buddyId;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            Field<StringGraphType>(
                "removeBuddy",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "buddyId" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        var buddyId = context.GetArgument<string>("buddyId");
                        userService.RemoveBuddy(userId, buddyId);
                        return buddyId;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

            Field<StringGraphType>(
                "uploadProfilePicture",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "imageData" }),
                resolve: context =>
                {
                    try
                    {
                        var userId = context.GetArgument<string>("userId");
                        var imageData = context.GetArgument<string>("imageData");
                        userService.UploadProfilePicture(userId, imageData);
                        return userId;
                    }
                    catch (Exception e)
                    {
                        context.Errors.Add(new ExecutionError(e.Message, e));
                        return null;
                    }
                }
            );

        }
    }
}