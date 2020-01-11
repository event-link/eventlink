using EventLink.Crawler.Client;
using EventLink.DataAccess.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventLink.Crawler.Parser
{
    public class EventParser : IEventParser
    {
        private static readonly Lazy<EventParser> instance =
            new Lazy<EventParser>(() => new EventParser());

        public static EventParser Instance => instance.Value;

        private EventParser()
        {

        }

        public IEnumerable<Event> ParseEventData(Provider provider, JObject json)
        {
            switch (provider)
            {
                case Provider.TicketMaster:
                    return ParseTicketMasterEventData(json);
                case Provider.Eventful:
                    return ParseEventfulData(json);
                default:
                    throw new CrawlerParserException("Invalid provider.");
            }
        }

        /****************************************************************
         * TICKETMASTER METHODS
         ****************************************************************/
        private IEnumerable<Event> ParseTicketMasterEventData(JObject json)
        {
            var tmEventList = json.SelectTokens("events[*]").ToList();
            var eventList = new JArray();

            /* For loop to iterate through events from TicketMaster one at a time. */
            foreach (var tmEvent in tmEventList)
            {
                var customEvent = new JObject
                {
                    /* Data from TicketMaster - Events. */
                    CreateJProperty("providerEventId", "id", tmEvent),
                    new JProperty("providerName", "TicketMaster"),
                    CreateJProperty("name", "name", tmEvent),
                    CreateJProperty("type", "type", tmEvent),
                    CreateJProperty("url", "url", tmEvent),
                    CreateJProperty("locale", "locale", tmEvent),

                    /* Creates custom EventLink input. */
                    new JProperty("description", ""),
                    new JProperty("isActive", true),
                    new JProperty("dbCreatedDate", new DateTime(1990, 1, 1)),
                    new JProperty("dbModifiedDate", new DateTime(1990, 1, 1)),
                    new JProperty("dbDeactivatedDate", new DateTime(1990, 1, 1)),
                    new JProperty("dbActivatedDate", new DateTime(1990, 1, 1)),
                    new JProperty("IsDeleted", false)
                };

                /* Creates custom object to store nested info from TicketMaster - Sales. */
                var sales = new JObject()
                {
                    CreateJProperty("startDateTime", "sales.public.startDateTime", tmEvent),
                    CreateJProperty("startTBD", "sales.public.startTBD", tmEvent),
                    CreateJProperty("endDateTime", "sales.public.endDateTime", tmEvent)
                };
                customEvent["sales"] = sales;

                /* Creates custom object to store nested info from TicketMaster - Dates. */
                var dates = new JObject()
                {
                    CreateJProperty("localStartDate", "dates.start.localDate", tmEvent),
                    CreateJProperty("timezone", "dates.timezone", tmEvent),
                    CreateJProperty("statusCode", "dates.status.code", tmEvent),
                    CreateJProperty("spanMultipleDays", "dates.spanMultipleDays", tmEvent),
                };
                customEvent["dates"] = dates;

                /* Creates custom object to store nested info from TicketMaster - Classifications. */
                var classifications = new JArray();
                var classIds = new[] { "segment", "genre", "subGenre" };
                var tokens = tmEvent.SelectTokens("classifications[*]").ToList();

                foreach (var token in tokens)
                {
                    var classEntry = new JObject
                    {
                        CreateJProperty("primary", "primary", token),
                        CreateJProperty("family", "family", token)
                    };

                    foreach (var id in classIds)
                    {
                        var entry = new JObject()
                        {
                            CreateJProperty("id", id + ".id", token),
                            CreateJProperty("name", id + ".name", token),
                        };
                        classEntry[id] = entry;
                    }
                    classifications.Add(classEntry);
                }
                customEvent["classifications"] = classifications;

                /* Data from TicketMaster - Promoter. */
                var promoter = new JObject()
                {
                    CreateJProperty("id", "promoter.id", tmEvent),
                    CreateJProperty("name", "promoter.name", tmEvent),
                };
                customEvent["promoter"] = promoter;

                /* Data from TicketMaster - Price ranges. */
                var priceRanges = new JArray();
                var priceRangeTokens = tmEvent.SelectTokens("priceRanges[*]").ToList();

                foreach (var token in priceRangeTokens)
                {
                    var classEntry = new JObject()
                    {
                        CreateJProperty("type", "type", token),
                        CreateJProperty("currency", "currency", token),
                        CreateJProperty("min", "min", token),
                        CreateJProperty("max", "max", token)
                    };
                    priceRanges.Add(classEntry);
                }
                customEvent["priceRanges"] = priceRanges;

                /* Data from TicketMaster - Venues. */
                var venues = new JArray();
                var venueTokens = tmEvent.SelectTokens("_embedded.venues[*]").ToList();

                foreach (var token in venueTokens)
                {
                    var venueEntry = new JObject()
                    {
                        CreateJProperty("id", "id", token),
                        CreateJProperty("name", "name", token),
                        CreateJProperty("type", "type", token),
                        CreateJProperty("url", "url", token),
                        CreateJProperty("locale", "locale", token),
                        CreateJProperty("timezone", "timezone", token),
                        CreateJProperty("city", "type", token),
                    };

                    var city = new JObject()
                    {
                        CreateJProperty("name", "city.name", token),
                    };
                    venueEntry["city"] = city;

                    var country = new JObject()
                    {
                        CreateJProperty("name", "country.name", token),
                        CreateJProperty("code", "country.countryCode", token)
                    };
                    venueEntry["country"] = country;

                    var address = new JObject()
                    {
                        CreateJProperty("line", "address.line1", token),
                    };
                    venueEntry["address"] = address;

                    venues.Add(venueEntry);
                }
                customEvent["venues"] = venues;

                /* Data from TicketMaster - Attractions. */
                var attractions = new JArray();
                var externalLinkIds = new string[] { "youtube", "twitter", "itunes", "lastfm", "facebook", "wiki", "instagram", "homepage" };
                var attractionTokens = tmEvent.SelectTokens("_embedded.attractions[*]").ToList();

                foreach (var token in attractionTokens)
                {
                    var attractionEntry = new JObject()
                    {
                        CreateJProperty("id", "id", token),
                        CreateJProperty("name", "name", token),
                        CreateJProperty("type", "type", token),
                        CreateJProperty("locale", "locale", token),
                    };

                    var externalLinks = new JObject();

                    foreach (var id in externalLinkIds)
                    {
                        var jArr = GetExternalLinkUrls(token, id);
                        if (jArr.Count > 0)
                        {
                            externalLinks[id] = jArr;
                        }
                    }

                    attractionEntry["externalLinks"] = externalLinks;

                    attractions.Add(attractionEntry);
                }
                customEvent["attractions"] = attractions;

                /* Data from TicketMaster - Images. */
                var images = new JArray();
                var imageTokens = tmEvent.SelectTokens("images[*]").ToList();

                foreach (var imageToken in imageTokens)
                {
                    var image = new JObject()
                    {
                        CreateJProperty("url", "url", imageToken),
                        CreateJProperty("ratio", "ratio", imageToken),
                        CreateJProperty("width", "width", imageToken),
                        CreateJProperty("height", "height", imageToken)
                    };
                    images.Add(image);
                }
                customEvent["images"] = images;

                /* Adds everything to the JObject. */
                eventList.Add(customEvent);
            }

            var eventObjList = new List<Event>();

            foreach (var jsonObj in eventList)
            {
                var eventObj = jsonObj.ToObject<Event>();
                eventObjList.Add(eventObj);
            }

            return eventObjList;
        }

        private IEnumerable<Event> ParseEventfulData(JObject json)
        {
            var efEventList = json.SelectTokens("events[*]").ToList();
            var eventList = new JArray();

            /* For loop to iterate through events from Eventful one at a time. */
            foreach (var efEvent in efEventList)
            {
                var customEvent = new JObject
                {
                    /* Data from EventfulLink - Events. */
                    CreateJProperty("providerEventId", "id", efEvent),
                    new JProperty("providerName", "Eventful"),
                    CreateJProperty("name", "title", efEvent),
                    new JProperty("type", "event"),
                    CreateJProperty("url", "url", efEvent),
                    new JProperty("locale", ""),

                    /* Creates custom EventLink input. */
                    CreateJProperty("description", "description", efEvent),
                    new JProperty("isActive", true),
                    new JProperty("dbCreatedDate", new DateTime(1990, 1, 1)),
                    new JProperty("dbModifiedDate", new DateTime(1990, 1, 1)),
                    new JProperty("dbDeactivatedDate", new DateTime(1990, 1, 1)),
                    new JProperty("dbActivatedDate", new DateTime(1990, 1, 1)),
                    new JProperty("IsDeleted", false)
                };

                /* Creates custom object to store nested info from Eventful. */
                var sales = new JObject()
                {
                    CreateJProperty("startDateTime", "start_time", efEvent),
                    new JProperty("startTBD", false),
                    CreateJProperty("endDateTime", "stop_time", efEvent),

                };
                customEvent["sales"] = sales;

                /* Creates custom object to store nested info from Eventful. */
                var dates = new JObject()
                {
                    CreateJProperty("localStartDate", "start_time", efEvent),
                    CreateJProperty("timezone", "olson_path", efEvent),
                    new JProperty("statusCode", "onsale"),
                };

                if (efEvent.SelectToken("recur_string").ToString().Contains("various"))
                {
                    dates.Add(new JProperty("spanMultipleDays", true));
                }
                else
                {
                    dates.Add(new JProperty("spanMultipleDays", false));
                }

                customEvent["dates"] = dates;

                /* Creates custom object to store nested info from Eventful. */
                var classifications = new JArray();
                var tokens = efEvent.SelectTokens("performers[*]").ToList();

                foreach (var token in tokens)
                {
                    var classEntry = new JObject
                    {
                    new JProperty("primary", true),
                    new JProperty("family", true)
                    };

                    var genre = new JObject()
                    {
                        CreateJProperty("id", "id", token),
                        CreateJProperty("name", "short_bio", token)
                    };
                    classEntry["genre"] = genre;

                    var subGenre = new JObject();
                    classEntry["subGenre"] = subGenre;

                    var segment = new JObject();
                    classEntry["segment"] = segment;

                    classifications.Add(classEntry);
                }
                customEvent["classifications"] = classifications;

                /* Data from Eventful - Promoter. */
                var promoter = new JObject()
                {
                    new JProperty("id", ""),
                    CreateJProperty("name", "owner", efEvent),
                };
                customEvent["promoter"] = promoter;

                /* PriceRanges is from Eventful. */
                customEvent["priceRanges"] = new JArray();

                /* Data from Eventful - Venues. */
                var venues = new JArray();

                var venueEntry = new JObject()
                    {
                        CreateJProperty("id", "venue_id", efEvent),
                        CreateJProperty("name", "venue_name", efEvent),
                        new JProperty("type", "venue"),
                        CreateJProperty("url", "venue_url", efEvent),
                        CreateJProperty("locale", "tz_city", efEvent),
                        new JProperty("timezone", ""),
                        CreateJProperty("city", "tz_city", efEvent),
                    };

                var city = new JObject()
                    {
                        CreateJProperty("name", "tz_city", efEvent),
                    };
                venueEntry["city"] = city;

                var country = new JObject()
                    {
                        CreateJProperty("name", "country_name", efEvent),
                        CreateJProperty("code", "country_abbr", efEvent)
                    };
                venueEntry["country"] = country;

                var address = new JObject()
                {
                    CreateJProperty("line", "venue_address", efEvent),
                };
                venueEntry["address"] = address;

                venues.Add(venueEntry);

                customEvent["venues"] = venues;

                /* Data from Eventful - Attractions. */
                var attractions = new JArray();
                customEvent["attractions"] = new JArray();

                /* Data from TicketMaster - Images. */
                var images = new JArray();
                var imageToken = efEvent.SelectToken("image");

                var smallImage = new JObject()
                {
                    new JProperty("url", ParseEventfulImageUrl("small.url", imageToken)),
                    CreateJProperty("width", "small.width", imageToken),
                    CreateJProperty("height", "small.height", imageToken),
                    new JProperty("ratio", "")
                };

                images.Add(smallImage);

                var mediumImage = new JObject()
                {
                    new JProperty("url", ParseEventfulImageUrl("medium.url", imageToken)),
                    CreateJProperty("width", "medium.width", imageToken),
                    CreateJProperty("height", "medium.height", imageToken),
                    new JProperty("ratio", "")
                };
                images.Add(mediumImage);

                customEvent["images"] = images;

                /* Adds everything to the JObject. */
                eventList.Add(customEvent);
            }

            var eventObjList = new List<Event>();

            foreach (var jsonObj in eventList)
            {
                var eventObj = jsonObj.ToObject<Event>();
                eventObjList.Add(eventObj);
            }

            return eventObjList;
        }

        private static JProperty CreateJProperty(string key, string query, JToken eventObj)
        {
            var value = eventObj.SelectToken(query);
            var jProperty = new JProperty(key, "");

            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                jProperty = new JProperty(key, value);
            }

            return jProperty;
        }

        private static JArray GetExternalLinkUrls(JToken token, string externalLinkType)
        {
            var links = new JArray();
            var linkTokens = token.SelectTokens("externalLinks." + externalLinkType + "[*]").ToList();
            foreach (var linkToken in linkTokens)
            {
                links.Add(new JObject(new JProperty("url", linkToken.SelectToken("url"))));
            };
            return links;
        }

        private static string ParseEventfulImageUrl(string query, JToken imageToken)
        {
            if (imageToken?.SelectToken(query) == null || string.IsNullOrEmpty(imageToken.SelectToken(query).Value<string>()))
            {
                return "";
            }

            var imageUrl = imageToken.SelectToken(query).Value<string>();

            if (imageUrl[0] == '/' && imageUrl[1] == '/')
            {
                imageUrl = "http:" + imageUrl;
            }

            return imageUrl;
        }

    }

}