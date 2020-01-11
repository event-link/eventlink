using EventLink.DataAccess.Models;
using EventLink.DataAccess.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventLink.DataAccess.UnitTest.Services
{
    [TestClass]
    public class EventServiceUnitTests
    {
        private static readonly EventService eventService = EventService.Instance;

        [TestInitialize()]
        public void Initialize()
        {
            var testEvents = GetTestEvents();

            foreach (var e in testEvents)
            {
                eventService.CreateEvent(e);
            }
        }

        [TestCleanup()]
        public void Cleanup()
        {
            var testEvents = GetTestEvents();

            /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
            if (SharedConstants.UnitTestMode)
            {
                foreach (var e in testEvents)
                {
                    eventService.DestroyEvent(e.Id);
                }
            }
        }

        /********************************************************
         * GetDocument METHOD TESTS
         ********************************************************/
        [TestMethod]
        public void GetDocument_FoundDoc()
        {
            Event e = null;
            Event e1 = null;

            try
            {
                e = eventService.GetEvent("507f191e810c19729de860ea");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                e1 = eventService.GetEvent("507f191e810c19729de860ae");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsNotNull(e);
            Assert.IsNotNull(e1);
        }

        [TestMethod]
        public void GetDocument_IdNull()
        {
            try
            {
                eventService.GetEvent(null);
            }
            catch (DANullOrEmptyIdException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /*
         * We thought that MongoDB would throw a MongoDBException, but instead
         * if MongoDB does not recognize the given Id, it just returns null.
         */
        [TestMethod]
        public void GetDocument_IdNotFound()
        {
            try
            {
                eventService.GetEvent("507f191e810c19729de860ff");
            }
            catch (DADocNotFoundException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
         * GetDocuments METHOD TESTS
         ********************************************************/
        [TestMethod]
        public void GetDocuments_FoundDocList()
        {
            IEnumerable<Event> eventDocList = null;

            try
            {
                eventDocList = eventService.GetEvents();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            Assert.IsNotNull(eventDocList);
            Assert.IsTrue(eventDocList.Any());
        }

        /*
         * If the database is empty, an empty list is returned.
         * No exceptions are thrown.
         */
        [TestMethod]
        public void GetDocuments_DocListNull()
        {
            /* Cleanup the test events such that the database is empty */
            Cleanup();

            IEnumerable<Event> eventDocList = null;

            try
            {
                eventDocList = eventService.GetEvents();
            }
            catch (DAException ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            Assert.IsNotNull(eventDocList);
            Assert.IsTrue(!eventDocList.Any());
        }

        /********************************************************
        * CreateDocument METHOD TESTS
        ********************************************************/
        [TestMethod]
        public void CreateDocument_DocCreated()
        {
            var e = new Event("507f191e810c19729de860ab", "TestProviderId3", "TicketMaster", "Sanne - The Musical - PREMIERE", "event", "https://www.ticketmaster.dk/event/sanne-the-musical-premiere-tickets/456563?language=en-us",
               "en-us", "", true,
               new Sales(new DateTime(2018, 11, 19), false, new DateTime(2019, 09, 19)),
               new Dates("2019-09-19", "Europe/Copenhagen", "onsale", false),
               new[]{new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1", "Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7vAve", "Musical")),
                new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1","Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7v7lt","Drama"))},
               new Promoter("1859", "Live Nation"),
               new[] { new PriceRange("standard including fees", "DKK", 280.0, 665.0),
                new PriceRange("standard", "DKK", 225.0, 625.0) },
               new[]{new Venue("Z598xZC4Z6e77", "name", "venue", "https://www.ticketmaster.dk/venue/tivolis-koncertsal-kobenhavn-v-billetter/tko/284", "en-us", "Europe/Copenhagen",
                    new City("København V"),
                    new Country("Denmark", "DK"),
                    new Address("Vesterbrogade 3"))},
               new[]{new Attraction("K8vZ9179naf", "Sanne - The Musical", "attraction", "en-us",
                    new Externallinks(
                        new[] { new Youtube(null) },
                        new[] { new Twitter(null) },
                        new[] { new Itune(null) },
                        new[] { new Lastfm(null) },
                        new[] { new Facebook(null) },
                        new[] { new Wiki(null) },
                        new[] { new Instagram(null) },
                        new[] { new Homepage(null) }
                    ))},
               new[] { new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_TABLET_LANDSCAPE_16_9.jpg", "16_9", 1024, 576),
                    new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_RECOMENDATION_16_9.jpg", "16_9", 100, 56) });

            try
            {
                eventService.CreateEvent(e);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Event e1 = null;

            try
            {
                e1 = eventService.GetEvent("507f191e810c19729de860ab");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsNotNull(e1);

            /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
            if (SharedConstants.UnitTestMode)
            {
                eventService.DestroyEvent(e1);
            }
        }

        [TestMethod]
        public void CreateDocument_DocProviderEventIdNull()
        {
            var e = new Event("507f191e810c19729de860ab", null, "TicketMaster", "Sanne - The Musical - PREMIERE", "event", "https://www.ticketmaster.dk/event/sanne-the-musical-premiere-tickets/456563?language=en-us",
               "en-us", "", true,
               new Sales(new DateTime(2018, 11, 19), false, new DateTime(2019, 09, 19)),
               new Dates("2019-09-19", "Europe/Copenhagen", "onsale", false),
               new[]{new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1", "Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7vAve", "Musical")),
                new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1","Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7v7lt","Drama"))},
               new Promoter("1859", "Live Nation"),
               new[] { new PriceRange("standard including fees", "DKK", 280.0, 665.0),
                new PriceRange("standard", "DKK", 225.0, 625.0) },
               new[]{new Venue("Z598xZC4Z6e77", "name", "venue", "https://www.ticketmaster.dk/venue/tivolis-koncertsal-kobenhavn-v-billetter/tko/284", "en-us", "Europe/Copenhagen",
                    new City("København V"),
                    new Country("Denmark", "DK"),
                    new Address("Vesterbrogade 3"))},
               new[]{new Attraction("K8vZ9179naf", "Sanne - The Musical", "attraction", "en-us",
                    new Externallinks(
                        new[] { new Youtube(null) },
                        new[] { new Twitter(null) },
                        new[] { new Itune(null) },
                        new[] { new Lastfm(null) },
                        new[] { new Facebook(null) },
                        new[] { new Wiki(null) },
                        new[] { new Instagram(null) },
                        new[] { new Homepage(null) }
                    ))},
               new[] { new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_TABLET_LANDSCAPE_16_9.jpg", "16_9", 1024, 576),
                    new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_RECOMENDATION_16_9.jpg", "16_9", 100, 56) });

            try
            {
                eventService.CreateEvent(e);
            }
            catch (DANullOrEmptyIdException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void CreateDocument_DocAlreadyExists()
        {
            var e = new Event("507f191e810c19729de860ab", "TestProviderId1", "TicketMaster", "Sanne - The Musical - PREMIERE", "event", "https://www.ticketmaster.dk/event/sanne-the-musical-premiere-tickets/456563?language=en-us",
               "en-us", "", true,
               new Sales(new DateTime(2018, 11, 19), false, new DateTime(2019, 09, 19)),
               new Dates("2019-09-19", "Europe/Copenhagen", "onsale", false),
               new[]{new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1", "Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7vAve", "Musical")),
                new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1","Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7v7lt","Drama"))},
               new Promoter("1859", "Live Nation"),
               new[] { new PriceRange("standard including fees", "DKK", 280.0, 665.0),
                new PriceRange("standard", "DKK", 225.0, 625.0) },
               new[]{new Venue("Z598xZC4Z6e77", "name", "venue", "https://www.ticketmaster.dk/venue/tivolis-koncertsal-kobenhavn-v-billetter/tko/284", "en-us", "Europe/Copenhagen",
                    new City("København V"),
                    new Country("Denmark", "DK"),
                    new Address("Vesterbrogade 3"))},
               new[]{new Attraction("K8vZ9179naf", "Sanne - The Musical", "attraction", "en-us",
                    new Externallinks(
                        new[] { new Youtube(null) },
                        new[] { new Twitter(null) },
                        new[] { new Itune(null) },
                        new[] { new Lastfm(null) },
                        new[] { new Facebook(null) },
                        new[] { new Wiki(null) },
                        new[] { new Instagram(null) },
                        new[] { new Homepage(null) }
                    ))},
               new[] { new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_TABLET_LANDSCAPE_16_9.jpg", "16_9", 1024, 576),
                    new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_RECOMENDATION_16_9.jpg", "16_9", 100, 56) });

            try
            {
                eventService.CreateEvent(e);
            }
            catch (DADocAlreadyExistsException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void CreateDocument_CheckInvalidIdFormat()
        {
            var e = new Event("sdfdsfsdasd", "TestProviderIdTesttt", "TicketMaster", "Sanne - The Musical - PREMIERE", "event", "https://www.ticketmaster.dk/event/sanne-the-musical-premiere-tickets/456563?language=en-us",
                      "en-us", "", true,
                      new Sales(new DateTime(2018, 11, 19), false, new DateTime(2019, 09, 19)),
                      new Dates("2019-09-19", "Europe/Copenhagen", "onsale", false),
                      new[]{new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1", "Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7vAve", "Musical")),
                new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1","Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7v7lt","Drama"))},
                      new Promoter("1859", "Live Nation"),
                      new[] { new PriceRange("standard including fees", "DKK", 280.0, 665.0),
                new PriceRange("standard", "DKK", 225.0, 625.0) },
                      new[]{new Venue("Z598xZC4Z6e77", "name", "venue", "https://www.ticketmaster.dk/venue/tivolis-koncertsal-kobenhavn-v-billetter/tko/284", "en-us", "Europe/Copenhagen",
                    new City("København V"),
                    new Country("Denmark", "DK"),
                    new Address("Vesterbrogade 3"))},
                      new[]{new Attraction("K8vZ9179naf", "Sanne - The Musical", "attraction", "en-us",
                    new Externallinks(
                        new[] { new Youtube(null) },
                        new[] { new Twitter(null) },
                        new[] { new Itune(null) },
                        new[] { new Lastfm(null) },
                        new[] { new Facebook(null) },
                        new[] { new Wiki(null) },
                        new[] { new Instagram(null) },
                        new[] { new Homepage(null) }
                    ))},
                      new[] { new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_TABLET_LANDSCAPE_16_9.jpg", "16_9", 1024, 576),
                    new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_RECOMENDATION_16_9.jpg", "16_9", 100, 56) });

            try
            {
                eventService.CreateEvent(e);
            }
            catch (DAInvalidIdException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
        * UpdateDocument METHOD TESTS
        ********************************************************/
        [TestMethod]
        public void UpdateDocument_DocWasUpdated()
        {
            var e = eventService.GetEvent("507f191e810c19729de860ae");

            var descExpected = "This is a test update. (Desc)";
            var nameExpected = "This is a test update. (Name)";
            var modifiedBefore = e.DbModifiedDate;

            e.Description = descExpected;
            e.Name = nameExpected;

            try
            {
                eventService.UpdateEvent(e);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var eNew = eventService.GetEvent(e.Id);

            Assert.IsNotNull(eNew);
            Assert.AreEqual(descExpected, eNew.Description);
            Assert.AreEqual(nameExpected, eNew.Name);
            Assert.AreNotEqual(modifiedBefore, eNew.DbModifiedDate);
            Assert.AreEqual(e.ProviderEventId, eNew.ProviderEventId);
            Assert.AreEqual(e.Id, eNew.Id);
            Assert.AreEqual(e.Locale, eNew.Locale);
        }

        [TestMethod]
        public void UpdateDocument_DocWasNotFound()
        {
            var e = new Event("507f191e810c19729de860cc", "TestId2005", "TicketMaster", "Sanne - The Musical - PREMIERE", "event", "https://www.ticketmaster.dk/event/sanne-the-musical-premiere-tickets/456563?language=en-us",
    "en-us", "", true,
    new Sales(new DateTime(2018, 11, 19), false, new DateTime(2019, 09, 19)),
    new Dates("2019-09-19", "Europe/Copenhagen", "onsale", false),
    new[]{new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1", "Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7vAve", "Musical")),
                new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1","Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7v7lt","Drama"))},
    new Promoter("1859", "Live Nation"),
    new[] { new PriceRange("standard including fees", "DKK", 280.0, 665.0),
                new PriceRange("standard", "DKK", 225.0, 625.0) },
    new[]{new Venue("Z598xZC4Z6e77", "name", "venue", "https://www.ticketmaster.dk/venue/tivolis-koncertsal-kobenhavn-v-billetter/tko/284", "en-us", "Europe/Copenhagen",
                    new City("København V"),
                    new Country("Denmark", "DK"),
                    new Address("Vesterbrogade 3"))},
    new[]{new Attraction("K8vZ9179naf", "Sanne - The Musical", "attraction", "en-us",
                    new Externallinks(
                        new[] { new Youtube(null) },
                        new[] { new Twitter(null) },
                        new[] { new Itune(null) },
                        new[] { new Lastfm(null) },
                        new[] { new Facebook(null) },
                        new[] { new Wiki(null) },
                        new[] { new Instagram(null) },
                        new[] { new Homepage(null) }
                    ))},
    new[] { new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_TABLET_LANDSCAPE_16_9.jpg", "16_9", 1024, 576),
                    new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_RECOMENDATION_16_9.jpg", "16_9", 100, 56) });

            try
            {
                eventService.UpdateEvent(e);
            }
            catch (DADocNotFoundException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
         * DELETE METHOD TESTS
         ********************************************************/
        /*
         * DeleteDocument does not delete the actual file in the database.
         * It sets the "isDeleted" value to true on the object itself.
         */
        [TestMethod]
        public void DeleteDocument_DocWasDeleted()
        {
            // Get event
            var e = eventService.GetEvent("507f191e810c19729de860ae");

            // delete event
            try
            {
                eventService.DeleteEvent(e.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            var eNew = eventService.GetEvent("507f191e810c19729de860ae");

            Assert.IsNotNull(eNew);
            Assert.AreNotEqual(e.DbDeletedDate, eNew.DbDeletedDate);
            Assert.AreEqual(true, eNew.IsDeleted);
            Assert.AreNotEqual(e.IsDeleted, eNew.IsDeleted);
        }

        [TestMethod]
        public void DeleteDocument_DocIsNull()
        {
            try
            {
                eventService.DeleteEvent((Event)null);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void DeleteDocument_DocIdIsNull()
        {
            try
            {
                eventService.DeleteEvent((string)null);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void DeleteDocument_DocNotFoundWrongId()
        {
            var e = new Event("507f191e810c19729de860cc", "TestProviderId1", "TicketMaster", "Sanne - The Musical - PREMIERE", "event", "https://www.ticketmaster.dk/event/sanne-the-musical-premiere-tickets/456563?language=en-us",
"en-us", "", true,
new Sales(new DateTime(2018, 11, 19), false, new DateTime(2019, 09, 19)),
new Dates("2019-09-19", "Europe/Copenhagen", "onsale", false),
new[]{new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1", "Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7vAve", "Musical")),
                new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1","Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7v7lt","Drama"))},
new Promoter("1859", "Live Nation"),
new[] { new PriceRange("standard including fees", "DKK", 280.0, 665.0),
                new PriceRange("standard", "DKK", 225.0, 625.0) },
new[]{new Venue("Z598xZC4Z6e77", "name", "venue", "https://www.ticketmaster.dk/venue/tivolis-koncertsal-kobenhavn-v-billetter/tko/284", "en-us", "Europe/Copenhagen",
                    new City("København V"),
                    new Country("Denmark", "DK"),
                    new Address("Vesterbrogade 3"))},
new[]{new Attraction("K8vZ9179naf", "Sanne - The Musical", "attraction", "en-us",
                    new Externallinks(
                        new[] { new Youtube(null) },
                        new[] { new Twitter(null) },
                        new[] { new Itune(null) },
                        new[] { new Lastfm(null) },
                        new[] { new Facebook(null) },
                        new[] { new Wiki(null) },
                        new[] { new Instagram(null) },
                        new[] { new Homepage(null) }
                    ))},
new[] { new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_TABLET_LANDSCAPE_16_9.jpg", "16_9", 1024, 576),
                    new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_RECOMENDATION_16_9.jpg", "16_9", 100, 56) });

            try
            {
                eventService.DeleteEvent(e);
            }
            catch (DADocNotFoundException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }


        [TestMethod]
        public void DeleteDocument_DocNotFoundWrongProviderEventId()
        {
            var e = new Event("507f191e810c19729de860ae", "WrongId", "TicketMaster", "Sanne - The Musical - PREMIERE", "event", "https://www.ticketmaster.dk/event/sanne-the-musical-premiere-tickets/456563?language=en-us",
"en-us", "", true,
new Sales(new DateTime(2018, 11, 19), false, new DateTime(2019, 09, 19)),
new Dates("2019-09-19", "Europe/Copenhagen", "onsale", false),
new[]{new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1", "Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7vAve", "Musical")),
                new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1","Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7v7lt","Drama"))},
new Promoter("1859", "Live Nation"),
new[] { new PriceRange("standard including fees", "DKK", 280.0, 665.0),
                new PriceRange("standard", "DKK", 225.0, 625.0) },
new[]{new Venue("Z598xZC4Z6e77","name", "venue", "https://www.ticketmaster.dk/venue/tivolis-koncertsal-kobenhavn-v-billetter/tko/284", "en-us", "Europe/Copenhagen",
                    new City("København V"),
                    new Country("Denmark", "DK"),
                    new Address("Vesterbrogade 3"))},
new[]{new Attraction("K8vZ9179naf", "Sanne - The Musical", "attraction", "en-us",
                    new Externallinks(
                        new[] { new Youtube(null) },
                        new[] { new Twitter(null) },
                        new[] { new Itune(null) },
                        new[] { new Lastfm(null) },
                        new[] { new Facebook(null) },
                        new[] { new Wiki(null) },
                        new[] { new Instagram(null) },
                        new[] { new Homepage(null) }
                    ))},
new[] { new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_TABLET_LANDSCAPE_16_9.jpg", "16_9", 1024, 576),
                    new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_RECOMENDATION_16_9.jpg", "16_9", 100, 56) });

            try
            {
                eventService.DeleteEvent(e);
            }
            catch (DADocNotFoundException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
         * DestroyLog METHOD TESTS
         *********************************************************/
        [TestMethod]
        public void DestroyDocument_DestroySingleDoc()
        {
            try
            {
                /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
                if (SharedConstants.UnitTestMode)
                {
                    eventService.DestroyEvent("507f191e810c19729de860ea");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            try
            {
                eventService.GetEvent("507f191e810c19729de860ea");
            }
            catch (DADocNotFoundException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void DestroyDocument_DestroyNullDoc()
        {
            try
            {
                /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
                if (SharedConstants.UnitTestMode)
                {
                    eventService.DestroyEvent((Event)null);
                }
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void DestroyDocument_DestroyNullStringId()
        {
            try
            {
                /* ONLY EXECUTE THIS CODE IF IN UNIT TEST MODE! */
                if (SharedConstants.UnitTestMode)
                {
                    eventService.DestroyEvent((string)null);
                }
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
         * UTIL METHOD TESTS
         ********************************************************/
        [TestMethod]
        public void CheckEvent_NullDAObject()
        {
            try
            {
                eventService.CreateEvent(null);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        [TestMethod]
        public void CheckEvent_EssentialData_Name()
        {
            var e = new Event("507f191e810c19729de860dd", "asdf", "TicketMaster", null, "event", "https://www.ticketmaster.dk/event/sanne-the-musical-premiere-tickets/456563?language=en-us",
                "en-us", null, null, null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            try
            {
                eventService.CheckEssentialEventData(e);
            }
            catch (DAException)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("The test did not throw an exception when one was expected!");
        }

        /********************************************************
         * ASSISTANT METHODS
         ********************************************************/
        private static IEnumerable<Event> GetTestEvents()
        {
            var e = new Event("507f191e810c19729de860ea", "TestProviderId1", "TicketMaster",
                "GIGGS", "event", "https://www.ticketmaster.dk/event/giggs-tickets/471921?language=en-us", "en-us",
                "", true,
                new Sales(new DateTime(
                    2019, 08, 30, 9, 0, 0), false, new DateTime(2019, 11, 16, 19, 0, 0)),
                new Dates("2019-11-16", "Europe/Copenhagen", "onsale", false),

                new[]{new Classification(true, false, new Segment("KZFzniwnSyZfZ7v7nJ", "Music"),
                    new Genre("KnvZfZ7vAv1", "Hip-Hop/Rap"), new SubGenre("KZazBEonSMnZfZ7vkvl", "Hip-Hop/Rap")),
                    new Classification(false, false, new Segment("KZFzniwnSyZfZ7v7nJ", "Music"),
                        new Genre("KnvZfZ7vAvl", "Other"), new SubGenre("KZazBEonSMnZfZ7vk1I", "Other"))
                }, new Promoter("1859", "Live Nation"),
                null,
                new[]{new Venue("Z198xZC4Zee7", "name", "venue", "https://www.ticketmaster.dk/venue/amager-bio-kobenhavn-s-billetter/ama/256", "en-us", "Europe/Copenhagen",
                    new City("København S"), new Country("Denmark", "DK"), new Address("Øresundsvej 6"))},

                new[]{new Attraction("K8vZ917G-0V", "Giggs", "attraction", "en-us",
                    new Externallinks(new []{new Youtube("https://www.youtube.com/user/sn1giggs1") }, new []{new Twitter("https://twitter.com/officialgiggs"), }, null,
                        null, new []{new Facebook("https://www.facebook.com/Giggs-275761059818"), }, new []{new Wiki("https://en.wikipedia.org/wiki/Giggs_(rapper)"), },
                        new []{new Instagram("https://www.instagram.com/officialgiggs"), new Instagram("https://www.instagram.com/officialgiggs/"), }, new []{new Homepage("http://sn1giggs.com/"), }))},
                new[] { new Image("https://s1.ticketm.net/dam/a/ce6/b63aaa6b-6411-4c8a-a263-587cd2c33ce6_1137601_RETINA_PORTRAIT_16_9.jpg", "16_9", 640, 360),
                    new Image("https://s1.ticketm.net/dam/a/ce6/b63aaa6b-6411-4c8a-a263-587cd2c33ce6_1137601_TABLET_LANDSCAPE_3_2.jpg", "3_2", 1024, 683),
                    new Image("https://s1.ticketm.net/dam/a/ce6/b63aaa6b-6411-4c8a-a263-587cd2c33ce6_1137601_RETINA_LANDSCAPE_16_9.jpg", "16_9", 1136, 639),
                });

            var e1 = new Event("507f191e810c19729de860ae", "TestProviderId2", "TicketMaster", "Sanne - The Musical - PREMIERE", "event", "https://www.ticketmaster.dk/event/sanne-the-musical-premiere-tickets/456563?language=en-us",
                "en-us", "", true,
                new Sales(new DateTime(2018, 11, 19), false, new DateTime(2019, 09, 19)),
                new Dates("2019-09-19", "Europe/Copenhagen", "onsale", false),
                new[]{new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1", "Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7vAve", "Musical")),
                new Classification(true, false,
                    new Segment("KZFzniwnSyZfZ7v7na", "Arts & Theatre"),
                    new Genre("KnvZfZ7v7l1","Theatre"),
                    new SubGenre("KZazBEonSMnZfZ7v7lt","Drama"))},
                new Promoter("1859", "Live Nation"),
                new[] { new PriceRange("standard including fees", "DKK", 280.0, 665.0),
                new PriceRange("standard", "DKK", 225.0, 625.0) },
                new[]{new Venue("Z598xZC4Z6e77", "name", "venue", "https://www.ticketmaster.dk/venue/tivolis-koncertsal-kobenhavn-v-billetter/tko/284", "en-us", "Europe/Copenhagen",
                    new City("København V"),
                    new Country("Denmark", "DK"),
                    new Address("Vesterbrogade 3"))},
                new[]{new Attraction("K8vZ9179naf", "Sanne - The Musical", "attraction", "en-us",
                    new Externallinks(
                        new[] { new Youtube(null) },
                        new[] { new Twitter(null) },
                        new[] { new Itune(null) },
                        new[] { new Lastfm(null) },
                        new[] { new Facebook(null) },
                        new[] { new Wiki(null) },
                        new[] { new Instagram(null) },
                        new[] { new Homepage(null) }
                    ))},
                new[] { new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_TABLET_LANDSCAPE_16_9.jpg", "16_9", 1024, 576),
                    new Image("https://s1.ticketm.net/dam/a/f5b/eca2a0d6-de21-4a23-ad59-cd9d6e371f5b_894331_RECOMENDATION_16_9.jpg", "16_9", 100, 56) });

            var list = new List<Event> { e, e1 };
            return list;
        }

    }

}