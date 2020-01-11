using EventLink.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace EventLink.API.Services
{
    public interface IEventService
    {
        Event GetEvent(string id);
        IEnumerable<Event> SearchEvents(string query, string filter);
    }

    public class EventService : IEventService
    {
        private static readonly Lazy<IEventService> instance =
            new Lazy<IEventService>(() => new EventService());

        public static IEventService Instance => instance.Value;

        private readonly DataAccess.Services.EventService _eventService = DataAccess.Services.EventService.Instance;

        private EventService()
        {

        }

        public Event GetEvent(string id)
        {
            try
            {
                return _eventService.GetEvent(id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public IEnumerable<Event> SearchEvents(string query, string filter)
        {
            try
            {
                return _eventService.SearchEvents(query, filter);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

    }
}