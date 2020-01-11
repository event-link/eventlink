using EventLink.Crawler.Client;
using EventLink.Crawler.Parser;
using EventLink.DataAccess.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace EventLink.Crawler.UnitTest
{
    [TestClass]
    public class EventParserTicketMasterTests
    {
        private static readonly LogService _logService = LogService.Instance;
        private static readonly IProviderClient _tmClient = ProviderClient.Instance;
        private static readonly IEventParser _parser = EventParser.Instance;

        private static JObject _jsonData;

        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            _jsonData = new JObject();
            var tmJson = _tmClient.GetEventData(Provider.TicketMaster, "DK");
            var tmEventList = tmJson.SelectTokens("events[*]").ToList().Take(20);

            var arr = new JArray();

            foreach (var e in tmEventList)
            {
                arr.Add(e);
            }

            _jsonData["events"] = arr;
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            /*
             * Since the Crawler is run once during these tests,
             * logs will be produced. We want to delete all of those from the
             * database, since this will cause problems with other tests.
             */
            var logList = _logService.GetLogs(LogDb.System).ToList();
            foreach (var log in logList)
            {
                _logService.DestroyLog(LogDb.System, (string)log.Id);
            }
        }

        [TestMethod]
        public void EventParser_SameDataLength()
        {
            /* Actual count of Event objects in the json */
            var expectedCount = _jsonData.SelectTokens("events[*]").ToList().Count;

            /* Parse the data */
            var parsedEvents = _parser.ParseEventData(Provider.TicketMaster, _jsonData);

            Assert.AreEqual(expectedCount, parsedEvents.Count());
        }

        [TestMethod]
        public void EventParser_SameProviderEventIds()
        {
            /* List of all ProviderEventIds from json */
            /* ProviderEventId is called "Id" in the original data from TicketMaster */
            var providerEventIds = _jsonData.SelectTokens("events[*].id").ToList();

            /* Parse the data */
            var parsedEvents = _parser.ParseEventData(Provider.TicketMaster, _jsonData).ToList();

            /* Check the length, just to be sure */
            Assert.AreEqual(providerEventIds.Count, parsedEvents.Count());

            for (var i = 0; i < providerEventIds.Count; i++)
            {
                var jsonId = providerEventIds[i].ToString();
                var parsedEventId = parsedEvents[i].ProviderEventId;

                if (!jsonId.Equals(parsedEventId))
                {
                    Assert.Fail("Two ProviderEventId's are not the same!: JSON id: "
                                 + jsonId + ", Parsed ProviderEventId: " + parsedEventId);
                }
            }
        }

        [TestMethod]
        public void EventParser_SameNames()
        {
            /* List of all Event names from json */
            var jsonNames = _jsonData.SelectTokens("events[*].name").ToList();

            /* Parse the data */
            var parsedEvents = _parser.ParseEventData(Provider.TicketMaster, _jsonData).ToList();

            /* Check the length, just to be sure */
            Assert.AreEqual(jsonNames.Count, parsedEvents.Count());

            for (var i = 0; i < jsonNames.Count; i++)
            {
                var jsonName = jsonNames[i].ToString();
                var parsedEventName = parsedEvents[i].Name;

                if (!jsonName.Equals(parsedEventName))
                {
                    Assert.Fail("Two Event names are not the same!: JSON name: "
                                + jsonName + ", Parsed Event name " + parsedEventName);
                }
            }
        }

        [TestMethod]
        public void EventParser_SameUrls()
        {
            /* List of all Event urls from json */
            var jsonUrls = _jsonData.SelectTokens("events[*].url").ToList();

            /* Parse the data */
            var parsedEvents = _parser.ParseEventData(Provider.TicketMaster, _jsonData).ToList();

            /* Check the length, just to be sure */
            Assert.AreEqual(jsonUrls.Count, parsedEvents.Count());

            for (var i = 0; i < jsonUrls.Count; i++)
            {
                var jsonUrl = jsonUrls[i].ToString();
                var parsedEventUrl = parsedEvents[i].Url;

                if (!jsonUrl.Equals(parsedEventUrl))
                {
                    Assert.Fail("Two Event Urls are not the same!: JSON url: "
                                + jsonUrl + ", Parsed Event url " + parsedEventUrl);
                }
            }
        }

        [TestMethod]
        public void EventParser_SameTypes()
        {
            /* List of all Event types from json */
            var jsonTypes = _jsonData.SelectTokens("events[*].type").ToList();

            /* Parse the data */
            var parsedEvents = _parser.ParseEventData(Provider.TicketMaster, _jsonData).ToList();

            /* Check the length, just to be sure */
            Assert.AreEqual(jsonTypes.Count, parsedEvents.Count());

            for (var i = 0; i < jsonTypes.Count; i++)
            {
                var jsonType = jsonTypes[i].ToString();
                var parsedEventType = parsedEvents[i].Type;

                if (!jsonType.Equals(parsedEventType))
                {
                    Assert.Fail("Two Event types are not the same!: JSON type: "
                                + jsonType + ", Parsed Event type " + parsedEventType);
                }
            }
        }

        [TestMethod]
        public void EventParser_SameDateLocalStartDate()
        {
            /* List of all Event localDates from json */
            var jsonLocalDates = _jsonData.SelectTokens("events[*].dates.start.localDate").ToList();

            /* Parse the data */
            var parsedEvents = _parser.ParseEventData(Provider.TicketMaster, _jsonData).ToList();

            /* Check the length, just to be sure */
            Assert.AreEqual(jsonLocalDates.Count, parsedEvents.Count());

            for (var i = 0; i < jsonLocalDates.Count; i++)
            {
                var jsonLocalDate = jsonLocalDates[i].ToString();
                var parsedEventLocalDate = parsedEvents[i].Dates.LocalStartDate;

                if (!jsonLocalDate.Equals(parsedEventLocalDate))
                {
                    Assert.Fail("Two Event localDates are not the same!: JSON localDate: "
                                + jsonLocalDate + ", Parsed Event localDate " + parsedEventLocalDate);
                }
            }
        }

        [TestMethod]
        public void EventParser_SamePriceRanges()
        {
            /* List of all Event PriceRanges from json */
            var jsonEvents = _jsonData.SelectTokens("events[*]").ToList();

            var jsonPriceRangeList = new List<List<JToken>>();

            /*
             * For every priceRange list in an array, add it to another list.
             */
            foreach (var tmEvent in jsonEvents)
            {
                var priceRangeList = tmEvent.SelectTokens("priceRanges[*]").ToList();
                jsonPriceRangeList.Add(priceRangeList);
            }

            /* Parse the data */
            var parsedEvents = _parser.ParseEventData(Provider.TicketMaster, _jsonData).ToList();

            /* Check the length, just to be sure */
            Assert.AreEqual(jsonPriceRangeList.Count, parsedEvents.Count());

            for (var i = 0; i < jsonPriceRangeList.Count; i++)
            {
                for (var j = 0; j < jsonPriceRangeList[i].Count; j++)
                {
                    var jsonMin = jsonPriceRangeList[i][j].SelectToken("min").ToString();
                    var parsedMin = parsedEvents[i].PriceRanges.ToList()[j].Min.ToString();

                    if (!jsonMin.Equals(parsedMin))
                    {
                        Assert.Fail("Two Event PriceRange min values are not the same!: JSON PriceRange min: "
                                    + jsonMin + ", Parsed PriceRange min: " + parsedMin);
                    }
                }
            }
        }

        [TestMethod]
        public void EventParser_SameSales()
        {
            /* List of all Event sales from json */
            var jsonSales = _jsonData.SelectTokens("events[*].sales.public").ToList();

            /* Parse the data */
            var parsedEvents = _parser.ParseEventData(Provider.TicketMaster, _jsonData).ToList();

            /* Check the length, just to be sure */
            Assert.AreEqual(jsonSales.Count, parsedEvents.Count());

            for (var i = 0; i < jsonSales.Count; i++)
            {
                var jsonStartDateTime = jsonSales[i].SelectToken("startDateTime").ToString();
                var jsonStartTBD = jsonSales[i].SelectToken("startTBD").ToString();
                var jsonEndDateTime = jsonSales[i].SelectToken("endDateTime").ToString();

                var parsedEventStartDateTime = parsedEvents[i].Sales.StartDateTime.ToString();
                var parsedEventStartTBD = parsedEvents[i].Sales.StartTBD.ToString();
                var parsedEventEndDateTime = parsedEvents[i].Sales.EndDateTime.ToString();

                if (!jsonStartDateTime.Equals(parsedEventStartDateTime))
                {
                    Assert.Fail("Two Event Sales StartDateTime are not the same!: JSON Sales StartDateTime: "
                                + jsonStartDateTime + ", Parsed Event Sales StartDateTime: " + parsedEventStartDateTime);
                }

                if (!jsonStartTBD.Equals(parsedEventStartTBD))
                {
                    Assert.Fail("Two Event Sales StartTBD are not the same!: JSON Sales StartTBD: "
                                + jsonStartTBD + ", Parsed Event Sales StartTBD: " + parsedEventStartTBD);
                }

                if (!jsonEndDateTime.Equals(parsedEventEndDateTime))
                {
                    Assert.Fail("Two Event Sales EndDateTime are not the same!: JSON Sales EndDateTime: "
                                + jsonEndDateTime + ", Parsed Event Sales EndDateTime: " + parsedEventEndDateTime);
                }
            }
        }

    }
}