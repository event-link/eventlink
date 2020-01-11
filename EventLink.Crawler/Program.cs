using EventLink.Crawler.Client;
using EventLink.Crawler.Parser;
using EventLink.Crawler.Util;
using EventLink.DataAccess;
using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using EventLink.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using Timer = System.Threading.Timer;


namespace EventLink.Crawler
{
    internal class Program
    {
        private static readonly Logger Log = Logger.Instance;
        private static readonly EventService EventService = EventService.Instance;
        private static readonly ProviderClient ProviderClient = ProviderClient.Instance;
        private static readonly EventParser Parser = EventParser.Instance;

        /*
         * A list of all the crawl jobs that are in use.
         * They might not be currently running.
         */
        private static readonly List<Timer> CrawlJobs = new List<Timer>();

        private static void Main()
        {
            Log.Log(LogDb.Event, "Program", "Main", "Crawler started.", LogLevel.Info, true);

            /* Update current events */
            UpdateCurrentEvents();

            /* Start the various crawl jobs */
            StartCrawlJobs();

            /* Pause the execution of the application */
            /*
             * TODO: Add some logic. If a timer dies, recreate it?
             */
            PauseExecution();
        }

        private static void UpdateCurrentEvents()
        {
            Log.Log(LogDb.Event, "Program", "UpdateCurrentEvents", "Updating current events...", LogLevel.Info, true);

            /* Get all events */
            var events = EventService.GetEvents();

            foreach (var e in events)
            {
                /*
                 * Check whether the event already has occured.
                 * If it has, set it inactive.
                 */
                if (e?.Sales?.EndDateTime < DateTime.Now)
                {
                    Log.Log(LogDb.Event, "Program", "UpdateCurrentEvents", "Event (" + e.Id + ") has expired and is now inactive.", LogLevel.Info, true);
                    e.IsActive = false;
                }

                /* Update the event */
                EventService.UpdateEvent(e);
            }

            Log.Log(LogDb.Event, "Program", "UpdateCurrentEvents", "Done updating current events.", LogLevel.Info, true);
        }

        private static void StartCrawlJobs()
        {
            /* Get individual timer parameters */
            var tmInterval = int.Parse(ConfigurationManager.AppSetting["Crawler:Interval:TicketMaster"]);
            var efInterval = int.Parse(ConfigurationManager.AppSetting["Crawler:Interval:Eventful"]);
            var countryCodes = ConfigurationManager.AppSetting["Crawler:CountryCodes"];

            Log.Log(LogDb.Event, "Program", "StartCrawlJobs", "Starting crawl jobs...", LogLevel.Info, true);

            /* TicketMaster crawl job */
            CrawlJobs.Add(StartCrawlJob(Provider.TicketMaster, tmInterval, countryCodes));

            /* Eventful crawl job */
            CrawlJobs.Add(StartCrawlJob(Provider.Eventful, efInterval, countryCodes));

        }

        private static Timer StartCrawlJob(Provider provider, int minuteInterval, string countryCodes)
        {
            Log.Log(LogDb.Event, "Program", "StartCrawlJob",
                "Starting crawl timer for " + provider + ". Interval (" + minuteInterval + "), CountryCodes (" + countryCodes + ").", LogLevel.Info, true);

            return new Timer((e) =>
            {
                CrawlEvents(provider, countryCodes);
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(minuteInterval));
        }

        private static void CrawlEvents(Provider provider, string countryCodes)
        {
            const string parentName = "Program";
            const string functionName = "CrawlEvents";
            var providerName = provider.ToString();

            var createCount = 0;
            var updateCount = 0;
            var errorCount = 0;

            Log.Log(LogDb.Event, parentName, functionName, "Started crawling " + providerName + " events...", LogLevel.Info, true);
            var eventJson = ProviderClient.GetEventData(provider, countryCodes);
            Log.Log(LogDb.Event, parentName, functionName, "Finished crawling " + providerName + " events.", LogLevel.Info, true);

            Log.Log(LogDb.Event, parentName, functionName, "Parsing " + providerName + " events...", LogLevel.Info, true);
            var events = Parser.ParseEventData(provider, eventJson);
            Log.Log(LogDb.Event, parentName, functionName, "Finished parsing " + providerName + " events.", LogLevel.Info, true);

            Log.Log(LogDb.Event, parentName, functionName, "Populating database with " + providerName + " event data...", LogLevel.Info, true);

            foreach (var e in events)
            {
                try
                {
                    EventService.CreateEvent(e);
                    createCount++;
                    Log.Log(LogDb.Event, parentName, functionName,
                        "Created Event with ProviderEventId (" + e.ProviderEventId + ") successfully.", LogLevel.Info, false);
                }
                catch (DADocAlreadyExistsException existsEx)
                {
                    /* If eventService throws DocAlreadyExists exception try to update instead. */
                    Log.Log(LogDb.Event, parentName, functionName, existsEx.Message, LogLevel.Info, false);

                    try
                    {
                        EventService.UpdateEvent(e);
                        updateCount++;
                        Log.Log(LogDb.Event, parentName, functionName,
                            "Event with Id (" + e.Id + "), ProviderEventId (" + e.ProviderEventId + ") was updated successfully.", LogLevel.Info, false);
                    }
                    catch (DAException updateEx)
                    {
                        /* If eventService throws another exception here, we don goofed. */
                        errorCount++;
                        Log.Log(LogDb.Event, parentName, functionName,
                            "Nested update threw exception: " + updateEx.Message + ". StackTrace: " + updateEx.StackTrace, LogLevel.Error, true);
                    }
                }
                catch (DAException createEx)
                {
                    /* If eventService throws another exception, log it below. */
                    errorCount++;
                    Log.Log(LogDb.Event, parentName, functionName,
                        "CreateDocument threw exception: " + createEx.Message + ". StackTrace: " + createEx.StackTrace, LogLevel.Error, true);
                }
            }

            Log.Log(LogDb.Statistics, parentName, functionName, providerName + " data population statistics: " +
                                              "Created (" + createCount + "), Updated (" + updateCount + "), Error (" + errorCount + ").", LogLevel.Info, true);
            Log.Log(LogDb.Event, parentName, functionName, "Finished populating database with " + providerName + " event data.", LogLevel.Info, true);
        }

        private static void PauseExecution()
        {
            while (true)
                Thread.Sleep(TimeSpan.FromMinutes(2).Milliseconds);
        }

    }
}