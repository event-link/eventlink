using EventLink.Crawler.Client;
using EventLink.DataAccess.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace EventLink.Crawler.Parser
{
    public interface IEventParser
    {
        IEnumerable<Event> ParseEventData(Provider provider, JObject json);
    }

    public class CrawlerParserException : Exception
    {
        public CrawlerParserException() { }
        public CrawlerParserException(string message) : base(message) { }
        public CrawlerParserException(string message, Exception inner) : base(message, inner) { }
    }

}
