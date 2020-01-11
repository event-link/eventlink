using EventLink.Crawler.Util;
using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using EventLink.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;

namespace EventLink.Crawler.Client
{
    public class ProviderClient : IProviderClient
    {
        private readonly string _ticketMasterEndPoint = ConfigurationManager.AppSetting["APIEndPoints:TicketMaster:Discovery"];
        private readonly string _eventfulEndPoint = ConfigurationManager.AppSetting["APIEndPoints:Eventful"];
        private readonly string _ticketMasterApiKey = ConfigurationManager.AppSetting["APIKeys:TicketMaster"];
        private readonly string _eventfulApiKey = ConfigurationManager.AppSetting["APIKeys:Eventful"];


        private static readonly Logger Logger = Logger.Instance;

        private static readonly Lazy<ProviderClient> instance =
            new Lazy<ProviderClient>(() => new ProviderClient());

        public static ProviderClient Instance => instance.Value;

        private ProviderClient()
        {

        }

        public JObject GetEventData(Provider provider)
        {
            return GetEventData(provider, "");
        }

        public JObject GetEventData(Provider provider, string countryCodes)
        {
            switch (provider)
            {
                case Provider.TicketMaster:
                    return GetTicketMasterEventData(countryCodes);
                case Provider.Eventful:
                    return GetEventfulData(countryCodes);
                default:
                    return null;
            }
        }

        private JObject GetTicketMasterEventData(string countryCodes)
        {
            Logger.Log(LogDb.Event, "ProviderClient", "GetTicketMasterEventData", "Getting TicketMaster event data...", LogLevel.Info, true);

            /* Initialize RestClient */
            var client = new RestClient(_ticketMasterEndPoint);

            if (client == null)
            {
                Logger.Log(LogDb.Event, "ProviderClient", "GetTicketMasterEventData", "RestClient object is null!", LogLevel.Error, true);
                throw new CrawlerClientException("RestClient object is null!");
            }

            var finalJson = new JObject();
            finalJson["events"] = new JArray();

            var requestPageNumber = 0;

            while (true)
            {
                var request = new RestRequest("events", Method.GET);

                /* Adds APIkey */
                request.AddParameter("apikey", _ticketMasterApiKey);

                /* Add specific countryCodes of which we want events from */
                if (!string.IsNullOrEmpty(countryCodes))
                {
                    request.AddParameter("countryCode", countryCodes);
                }

                /* Give us the total page amount of data, so we dont have to call the API many times */
                request.AddParameter("size", "200");

                /* Add current page level */
                request.AddParameter("page", requestPageNumber);

                /* Executes request and saves the result of the REST call */
                var response = client.Execute(request);

                if (response == null)
                {
                    Logger.Log(LogDb.Event, "ProviderClient", "GetTicketMasterEventData", "Response object is null!", LogLevel.Error, true);
                    throw new CrawlerClientException("Response object is null!");
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Logger.Log(LogDb.Event, "ProviderClient", "GetTicketMasterEventData", "Response HTTP StatusCode not OK: " + response.StatusCode, LogLevel.Error, true);
                    throw new CrawlerClientException("Reponse HTTP StatusCode not OK: " + response.StatusCode);
                }

                var result = response.Content;

                if (result == null)
                {
                    Logger.Log(LogDb.Event, "ProviderClient", "GetTicketMasterEventData", "Response result is null!", LogLevel.Error, true);
                    throw new CrawlerClientException("Response result is null!");
                }

                /* Converts it to a Json object */
                var json = JObject.Parse(result);

                if (json == null)
                {
                    Logger.Log(LogDb.Event, "ProviderClient", "GetTicketMasterEventData", "Parsing result to json failed!", LogLevel.Error, true);
                    throw new CrawlerClientException("Parsing result to json failed!");
                }

                var pageToken = json.SelectToken("page");

                if (pageToken?.SelectToken("totalPages") == null || pageToken.SelectToken("totalPages") == null)
                {
                    Logger.Log(LogDb.Event, "ProviderClient", "GetTicketMasterEventData", "A JSON page element is null!", LogLevel.Error, true);
                    throw new CrawlerClientException("A JSON page element is null!");
                }

                var totalPages = pageToken.SelectToken("totalPages").Value<int>();
                var pageNumber = pageToken.SelectToken("number").Value<int>();

                var finalArr = (JArray)finalJson["events"];

                foreach (var token in json.SelectToken("_embedded.events"))
                {
                    finalArr.Add(token);
                }
                finalJson["events"] = finalArr;

                if (totalPages - 1 == pageNumber)
                {
                    Logger.Log(LogDb.Event, "ProviderClient", "GetTicketMasterEventData", "Terminating at page: " + requestPageNumber, LogLevel.Info, true);
                    break;
                }

                Logger.Log(LogDb.Event, "ProviderClient", "GetTicketMasterEventData", "TicketMaster event page: " + requestPageNumber + ", total: " + totalPages, LogLevel.Info, true);

                requestPageNumber++;
            }

            Logger.Log(LogDb.Event, "ProviderClient", "GetTicketMasterEventData", "Finished getting TicketMaster data.", LogLevel.Info, true);

            return finalJson;
        }

        private JObject GetEventfulData(string countryCodes)
        {
            Logger.Log(LogDb.Event, "ProviderClient", "GetEventfulData", "Getting Eventful event data...", LogLevel.Info, true);

            /* Initialize RestClient */
            var client = new RestClient(_eventfulEndPoint);

            if (client == null)
            {
                Logger.Log(LogDb.Event, "ProviderClient", "GetEventfulData", "RestClient object is null!", LogLevel.Error, true);
                throw new CrawlerClientException("RestClient object is null!");
            }

            var finalJson = new JObject();
            finalJson["events"] = new JArray();

            var request = new RestRequest("search", Method.GET);

            /* Adds what query we're searching for */
            request.AddParameter("q", "event");

            /* Adds APIkey */
            request.AddParameter("app_key", _eventfulApiKey);

            /* Add specific countryCodes of which we want events from */
            if (!string.IsNullOrEmpty(countryCodes))
            {
                request.AddParameter("l", countryCodes);
            }

            /* Give us the total page amount of data, so we dont have to call the API many times */
            request.AddParameter("page_size", "1000000");

            /* Executes request and saves the result of the REST call */
            var response = client.Execute(request);

            if (response == null)
            {
                Logger.Log(LogDb.Event, "ProviderClient", "GetEventfulData", "Response object is null!", LogLevel.Error, true);
                throw new CrawlerClientException("Response object is null!");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Log(LogDb.Event, "ProviderClient", "GetEventfulData", "Response HTTP StatusCode not OK: " + response.StatusCode, LogLevel.Error, true);
                throw new CrawlerClientException("Reponse HTTP StatusCode not OK: " + response.StatusCode);
            }

            var result = response.Content;

            if (result == null)
            {
                Logger.Log(LogDb.Event, "ProviderClient", "GetEventfulData", "Response result is null!", LogLevel.Error, true);
                throw new CrawlerClientException("Response result is null!");
            }

            /* Converts it to a Json object */
            var json = JObject.Parse(result);

            if (json == null)
            {
                Logger.Log(LogDb.Event, "ProviderClient", "GetEventfulData", "Parsing result to json failed!", LogLevel.Error, true);
                throw new CrawlerClientException("Parsing result to json failed!");
            }

            var finalArr = (JArray)finalJson["events"];

            foreach (var token in json.SelectToken("events.event"))
            {
                finalArr.Add(token);
            }
            finalJson["events"] = finalArr;


            Logger.Log(LogDb.Event, "ProviderClient", "GetEventfulData", "Finished getting Eventful data.", LogLevel.Info, true);

            return finalJson;
        }
    }
}