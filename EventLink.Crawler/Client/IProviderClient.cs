using Newtonsoft.Json.Linq;
using System;

namespace EventLink.Crawler.Client
{
    public interface IProviderClient
    {
        JObject GetEventData(Provider provider);
        JObject GetEventData(Provider provider, string countryCodes);
    }

    public class CrawlerClientException : Exception
    {
        public CrawlerClientException() { }
        public CrawlerClientException(string message) : base(message) { }
        public CrawlerClientException(string message, Exception inner) : base(message, inner) { }
    }

    public enum Provider
    {
        TicketMaster = 100,
        Eventful = 200
    }

}