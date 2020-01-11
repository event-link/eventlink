using EventLink.DataAccess.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace EventLink.DataAccess.Services
{
    public interface IEventService
    {
        IEnumerable<Event> SearchEvents(string query, string filter);
        Event GetEvent(string id);
        IEnumerable<Event> GetEvents();
        void CreateEvent(Event eventObj);
        void UpdateEvent(Event eventObj);
        void DeleteEvent(Event eventObj);
        void DeleteEvent(string id);
        /* DestroyLog methods will DELETE data permanently. */
        /* These methods should ONLY be used for unit testing. Not in production. */
        void DestroyEvent(Event eventObj);
        void DestroyEvent(string id);
        void CheckNullEvent(Event eventObj);
        void CheckEssentialEventData(Event eventObj);
    }

    public class EventService : IEventService
    {
        private static readonly DbContext DbContext = DbContext.Instance;
        private readonly IMongoCollection<Event> _events;

        private static readonly Lazy<EventService> instance =
            new Lazy<EventService>(() => new EventService());

        public static EventService Instance => instance.Value;

        private EventService()
        {
            _events = DbContext.GetEventCollection();
        }

        public IEnumerable<Event> SearchEvents(string query, string filter)
        {
            IEnumerable<Event> searchResults;

            if (query == null)
            {
                query = "";
            }
            else
            {
                query = query.Replace(" ", "*");
            }

            try
            {
                var filterQuery = Builders<Event>.Filter.Regex("Name", new BsonRegularExpression(query, "i"));

                if (!string.IsNullOrEmpty(query))
                {
                    searchResults = _events.Find(filterQuery).SortBy(e => e.Name).ToList();
                }
                else
                {
                    var tmpResults = _events.Find(filterQuery).SortBy(e => e.Sales.EndDateTime).ToList();
                    var nullEndDateList = new List<Event>();

                    foreach (var e in tmpResults)
                    {
                        if (e.Sales.EndDateTime == null || string.IsNullOrEmpty(e.Sales.EndDateTime.ToString()))
                        {
                            nullEndDateList.Add(e);
                        }
                    }

                    foreach (var e in nullEndDateList)
                    {
                        tmpResults.Remove(e);
                    }

                    foreach (var e in nullEndDateList)
                    {
                        tmpResults.Add(e);
                    }

                    searchResults = tmpResults;
                }
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            if (searchResults == null)
            {
                throw new DAException("Something went wrong searching with query (" + query + ") and filter (" + filter + ")!");
            }

            return searchResults;
        }

        /*****************************
        * CRUD OPERATIONS
        *****************************/
        public Event GetEvent(string id)
        {
            Event eventDoc;

            /* If id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Tries to find the first object with the given id */
            try
            {
                eventDoc = _events.Find(e => e.Id == id).FirstOrDefault();
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            /*
             * If the Event document is null, then we assume that an Event with the given
             * id does not exist in the database.
             */
            if (eventDoc == null)
            {
                throw new DADocNotFoundException("Event with Id (" + id + ") was not found!");
            }

            return eventDoc;
        }

        public IEnumerable<Event> GetEvents()
        {
            IEnumerable<Event> eventDocList;

            /* Tries to find all Event documents */
            try
            {
                eventDocList = _events.Find("{}").ToList(); // {} has same effect as * in SQL (wildcard)
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            /* If the list is null, throw an exception */
            if (eventDocList == null)
            {
                throw new DADocNotFoundException("List of Event objects is null!");
            }

            return eventDocList;
        }

        public void CreateEvent(Event eventObj)
        {
            /* Validate the Event object */
            CheckNullEvent(eventObj);

            /* Checks if ProviderEventId has been set, since this is the ID we use to search for Events. */
            if (string.IsNullOrEmpty(eventObj.ProviderEventId))
            {
                throw new DANullOrEmptyIdException("ProviderEventId is null!");
            }

            CheckEssentialEventData(eventObj);

            try
            {
                /* If the Event Id already exists, throw exception */
                if (eventObj.Id != null && _events.Find(e => e.Id == eventObj.Id).FirstOrDefault() != null)
                {
                    throw new DADocAlreadyExistsException("Event with Id (" + eventObj.Id + "), ProviderEventId (" + eventObj.ProviderEventId + ") already exists!");
                }

                /* If the Event ProviderEventId already exists, throw exception */
                if (_events.Find(e => e.ProviderEventId == eventObj.ProviderEventId).FirstOrDefault() != null)
                {
                    throw new DADocAlreadyExistsException("Event with Id (" + eventObj.Id + "), ProviderEventId (" + eventObj.ProviderEventId + ") already exists!");
                }

                /* Creates a new Event document */
                eventObj.DbCreatedDate = DateTime.Now;
                eventObj.DbModifiedDate = DateTime.Now;
                _events.InsertOne(eventObj);
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + eventObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void UpdateEvent(Event eventObj)
        {
            /* Validate the Event object */
            CheckNullEvent(eventObj);
            CheckEssentialEventData(eventObj);

            /*
             * If both Id's are non-existent, we have no way to check for uniqueness in the database,
             * therefore are unable to update the given document and an exception will be thrown.
             */
            if (string.IsNullOrEmpty(eventObj.Id) && string.IsNullOrEmpty(eventObj.ProviderEventId))
            {
                throw new DANullOrEmptyIdException("Event Id and ProviderEventId are null or empty!");
            }

            /*
             * If the eventObj.Id is non-existent, but we have the ProviderEventId,
             * we just use the ProviderEventId for uniqueness based on the update and
             * give the object a new unique id.
             */
            if (string.IsNullOrEmpty(eventObj.Id) && !string.IsNullOrEmpty(eventObj.ProviderEventId))
            {
                try
                {
                    var dbObj = _events.Find(e => e.ProviderEventId == eventObj.ProviderEventId).FirstOrDefault();

                    if (dbObj == null)
                    {
                        throw new DADocNotFoundException("Event with Id (non-existent), ProviderEventId (" + eventObj.ProviderEventId + ") was not found!");
                    }

                    /*
                     * Since the eventObj is in this clause it has no id, therefore we need the Id from the Event in the database
                     * and add it to the event before we can update it.
                     */
                    eventObj.Id = dbObj.Id;

                    eventObj.DbModifiedDate = DateTime.Now;
                    _events.ReplaceOne(e => e.ProviderEventId == eventObj.ProviderEventId, eventObj);
                }
                catch (FormatException)
                {
                    throw new DAInvalidIdException("Id (" + eventObj.Id + ") has an invalid format!");
                }
                catch (MongoException e)
                {
                    /* If exception is thrown from DB, pass the exception up */
                    throw new DAException(e.Message, e);
                }
            }
            /*
             * However, if we have the eventObj.Id and not the eventObj.ProviderEventId (or if both are present), we just base the uniqueness 
             * on the eventObj.Id.
             */
            else if (!string.IsNullOrEmpty(eventObj.Id) && string.IsNullOrEmpty(eventObj.ProviderEventId) ||
                     !string.IsNullOrEmpty(eventObj.Id) && !string.IsNullOrEmpty(eventObj.ProviderEventId))
            {
                try
                {
                    if (_events.Find(e => e.Id == eventObj.Id).FirstOrDefault() == null)
                    {
                        throw new DADocNotFoundException("Event with Id (" + eventObj.Id + "), ProviderEventId (non-existent) was not found!");
                    }

                    /*
                     * Check whether the event already has occured.
                     * If it has, set it inactive.
                     */
                    if (eventObj.Sales.EndDateTime < DateTime.Now)
                    {
                        eventObj.IsActive = false;
                    }

                    eventObj.DbModifiedDate = DateTime.Now;
                    _events.ReplaceOne(e => e.Id == eventObj.Id, eventObj);
                }
                catch (FormatException)
                {
                    throw new DAInvalidIdException("Id (" + eventObj.Id + ") has an invalid format!");
                }
                catch (MongoException e)
                {
                    /* If exception is thrown from DB, pass the exception up */
                    throw new DAException(e.Message, e);
                }
            }
        }

        public void DeleteEvent(Event eventObj)
        {
            /* Validate the object */
            CheckNullEvent(eventObj);

            /* Checks whether the Event document exists */
            try
            {
                if (_events.Find(e => e.Id == eventObj.Id && e.ProviderEventId == eventObj.ProviderEventId).FirstOrDefault() == null)
                {
                    throw new DADocNotFoundException("Event with Id (" + eventObj.Id + "), ProviderEventId (" + eventObj.ProviderEventId + ") was not found!");
                }
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + eventObj.Id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }

            DeleteEvent(eventObj.Id);
        }

        public void DeleteEvent(string id)
        {
            /* If Event Id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Find the document in the database */
            var eventObj = GetEvent(id);

            /* Validate the object */
            CheckNullEvent(eventObj);

            /* Tries to set the Event inactive. */
            try
            {
                eventObj.IsDeleted = true;
                eventObj.DbDeletedDate = DateTime.Now;
                eventObj.DbModifiedDate = DateTime.Now;
                UpdateEvent(eventObj);
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void DestroyEvent(Event eventObj)
        {
            if (!SharedConstants.UnitTestMode)
            {
#pragma warning disable CS0162 // Unreachable code detected
                return;
#pragma warning restore CS0162 // Unreachable code detected
            }

            /* Validate the Event object */
            CheckNullEvent(eventObj);

            /* If Event Id is null or empty */
            if (string.IsNullOrEmpty(eventObj.Id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Deletes the document from the DB */
            DestroyEvent(eventObj.Id);
        }

        public void DestroyEvent(string id)
        {
            if (!SharedConstants.UnitTestMode)
            {
#pragma warning disable CS0162 // Unreachable code detected
                return;
#pragma warning restore CS0162 // Unreachable code detected
            }

            /* If Event Id is null or empty */
            if (string.IsNullOrEmpty(id))
            {
                throw new DANullOrEmptyIdException();
            }

            /* Deletes the document from the DB */
            try
            {
                _events.DeleteOne(e => e.Id == id);
            }
            catch (FormatException)
            {
                throw new DAInvalidIdException("Id (" + id + ") has an invalid format!");
            }
            catch (MongoException e)
            {
                /* If exception is thrown from DB, pass the exception up */
                throw new DAException(e.Message, e);
            }
        }

        public void CheckNullEvent(Event eventObj)
        {
            /* If Event object is null, throw exception */
            if (eventObj == null)
            {
                throw new DAException("Event object is null!");
            }
        }

        public void CheckEssentialEventData(Event eventObj)
        {
            /* Checks whether essential data for the Event object is intact */
            if (eventObj.Name == null)
            {
                throw new DAException("All or some essential Event data has not been found!");
            }
        }

    }
}