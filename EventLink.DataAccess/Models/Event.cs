using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EventLink.DataAccess.Models
{
    public class Event
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ProviderEventId")]
        public string ProviderEventId { get; set; }

        [BsonElement("ProviderName")]
        public string ProviderName { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("Url")]
        public string Url { get; set; }

        [BsonElement("Locale")]
        public string Locale { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Sales")]
        public Sales Sales { get; set; }

        [BsonElement("Dates")]
        public Dates Dates { get; set; }

        [BsonElement("Classifications")]
        public IEnumerable<Classification> Classifications { get; set; }

        [BsonElement("Promoter")]
        public Promoter Promoter { get; set; }

        [BsonElement("PriceRanges")]
        public IEnumerable<PriceRange> PriceRanges { get; set; }

        [BsonElement("Venues")]
        public IEnumerable<Venue> Venues { get; set; }

        [BsonElement("Attractions")]
        public IEnumerable<Attraction> Attractions { get; set; }

        [BsonElement("Images")]
        public IEnumerable<Image> Images { get; set; }

        [BsonElement("IsActive")]
        public bool? IsActive { get; set; }

        [BsonElement("DbCreatedDate")]
        public DateTime DbCreatedDate { get; set; }

        [BsonElement("DbModifiedDate")]
        public DateTime DbModifiedDate { get; set; }

        [BsonElement("DbDeletedDate")]
        public DateTime DbDeletedDate { get; set; }

        [BsonElement("DbReactivatedDate")]
        public DateTime DbReactivatedDate { get; set; }

        [BsonElement("IsDeleted")]
        public bool IsDeleted { get; set; }

        [JsonConstructor]
        public Event(string providerEventId, string providerName, string name, string type,
            string url, string locale, string description, bool? isActive, Sales sales,
            Dates dates, Classification[] classifications, Promoter promoter, PriceRange[] priceRanges,
            Venue[] venues, Attraction[] attractions, Image[] images)
        {
            ProviderEventId = providerEventId;
            ProviderName = providerName;
            Name = name;
            Type = type;
            Url = url;
            Locale = locale;
            Description = description;
            IsActive = isActive;
            Sales = sales;
            Dates = dates;
            Classifications = classifications;
            Promoter = promoter;
            PriceRanges = priceRanges;
            Venues = venues;
            Attractions = attractions;
            Images = images;
        }

        /* Constructor with Mongod Id */
        public Event(string id, string providerEventId, string providerName, string name, string type,
            string url, string locale, string description, bool? isActive, Sales sales,
            Dates dates, Classification[] classifications, Promoter promoter, PriceRange[] priceRanges,
            Venue[] venues, Attraction[] attractions, Image[] images)
        {
            Id = id;
            ProviderEventId = providerEventId;
            ProviderName = providerName;
            Name = name;
            Type = type;
            Url = url;
            Locale = locale;
            Description = description;
            IsActive = isActive;
            Sales = sales;
            Dates = dates;
            Classifications = classifications;
            Promoter = promoter;
            PriceRanges = priceRanges;
            Venues = venues;
            Attractions = attractions;
            Images = images;
        }

        public override string ToString()
        {
            return $"{nameof(ProviderEventId)}: {ProviderEventId}, {nameof(ProviderName)}: {ProviderName}, " +
                   $"{nameof(Name)}: {Name}, {nameof(Type)}: {Type}, {nameof(Url)}: {Url}, {nameof(Locale)}: {Locale}, " +
                   $"{nameof(Description)}: {Description}, {nameof(IsActive)}: {IsActive}, {nameof(Sales)}: {Sales}, " +
                   $"{nameof(Dates)}: {Dates}, {nameof(Classifications)}: {Classifications}, {nameof(Promoter)}: {Promoter}, " +
                   $"{nameof(PriceRanges)}: {PriceRanges}, {nameof(Venues)}: {Venues}, {nameof(Attractions)}: {Attractions}, " +
                   $"{nameof(Images)}: {Images}, {nameof(IsDeleted)}: {IsDeleted}, {nameof(DbCreatedDate)}: {DbCreatedDate}, " +
                   $"{nameof(DbModifiedDate)}: {DbModifiedDate}, {nameof(DbDeletedDate)}: {DbDeletedDate}, " +
                   $"{nameof(DbReactivatedDate)}: {DbReactivatedDate}";
        }

    }

    public class Sales
    {
        [BsonElement("StartDateTime")]
        public DateTime? StartDateTime { get; set; }

        [BsonElement("StartTBD")]
        public bool? StartTBD { get; set; }

        [BsonElement("EndDateTime")]
        public DateTime? EndDateTime { get; set; }

        public Sales(DateTime? startDateTime, bool? startTbd, DateTime? endDateTime)
        {
            StartDateTime = startDateTime;
            StartTBD = startTbd;
            EndDateTime = endDateTime;
        }

        public override string ToString()
        {
            return $"{nameof(StartDateTime)}: {StartDateTime}, {nameof(StartTBD)}: " +
                   $"{StartTBD}, {nameof(EndDateTime)}: {EndDateTime}";
        }

    }

    public class Dates
    {
        [BsonElement("LocalStartDate")]
        public string LocalStartDate { get; set; }

        [BsonElement("Timezone")]
        public string Timezone { get; set; }

        [BsonElement("StatusCode")]
        public string StatusCode { get; set; }

        [BsonElement("SpanMultipleDays")]
        public bool? SpanMultipleDays { get; set; }

        public Dates(string localStartDate, string timezone, string statusCode, bool? spanMultipleDays)
        {
            LocalStartDate = localStartDate;
            Timezone = timezone;
            StatusCode = statusCode;
            SpanMultipleDays = spanMultipleDays;
        }

        public override string ToString()
        {
            return $"{nameof(LocalStartDate)}: {LocalStartDate}, {nameof(Timezone)}: {Timezone}, " +
                   $"{nameof(StatusCode)}: {StatusCode}, {nameof(SpanMultipleDays)}: {SpanMultipleDays}";
        }

    }

    public class Promoter
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        public Promoter(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
        }

    }

    public class PriceRange
    {
        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("Currency")]
        public string Currency { get; set; }

        [BsonElement("Min")]
        public double? Min { get; set; }

        [BsonElement("Max")]
        public double? Max { get; set; }

        public PriceRange(string type, string currency, double? min, double? max)
        {
            Type = type;
            Currency = currency;
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, {nameof(Currency)}: {Currency}, " +
                   $"{nameof(Min)}: {Min}, {nameof(Max)}: {Max}";
        }

    }

    public class Classification
    {

        [BsonElement("Primary")]
        public bool? Primary { get; set; }

        [BsonElement("Family")]
        public bool? Family { get; set; }

        [BsonElement("Segment")]
        public Segment Segment { get; set; }

        [BsonElement("Genre")]
        public Genre Genre { get; set; }

        [BsonElement("SubGenre")]
        public SubGenre SubGenre { get; set; }

        public Classification(bool? primary, bool? family, Segment segment, Genre genre, SubGenre subGenre)
        {
            Primary = primary;
            Family = family;
            Segment = segment;
            Genre = genre;
            SubGenre = subGenre;
        }

        public override string ToString()
        {
            return $"{nameof(Primary)}: {Primary}, {nameof(Family)}: {Family}, " +
                   $" {nameof(Segment)}: {Segment}, {nameof(Genre)}: {Genre}, {nameof(SubGenre)}: {SubGenre}";
        }

    }

    public class Segment
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        public Segment(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
        }

    }

    public class Genre
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        public Genre(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
        }

    }

    public class SubGenre
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        public SubGenre(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
        }

    }

    public class Venue
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("Url")]
        public string Url { get; set; }

        [BsonElement("Locale")]
        public string Locale { get; set; }

        [BsonElement("Timezone")]
        public string Timezone { get; set; }

        [BsonElement("City")]
        public City City { get; set; }

        [BsonElement("Country")]
        public Country Country { get; set; }

        [BsonElement("Address")]
        public Address Address { get; set; }

        public Venue(string id, string name, string type, string url, string locale, string timezone, City city, Country country, Address address)
        {
            Id = id;
            Name = name;
            Type = type;
            Url = url;
            Locale = locale;
            Timezone = timezone;
            City = city;
            Country = country;
            Address = address;
        }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, {nameof(Id)}: {Id}," +
                   $" {nameof(Url)}: {Url}, {nameof(Locale)}: {Locale}, " +
                   $"{nameof(Timezone)}: {Timezone}, {nameof(City)}: {City}, " +
                   $"{nameof(Country)}: {Country}, {nameof(Address)}: {Address}";
        }

    }

    public class City
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        public City(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}";
        }

    }

    public class Country
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Code")]
        public string Code { get; set; }

        public Country(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Code)}: {Code}";
        }

    }

    public class Address
    {
        [BsonElement("Line")]
        public string Line { get; set; }

        public Address(string line)
        {
            Line = line;
        }

        public override string ToString()
        {
            return $"{nameof(Line)}: {Line}";
        }

    }

    public class Attraction
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("Locale")]
        public string Locale { get; set; }

        [BsonElement("Externallinks")]
        public Externallinks Externallinks { get; set; }

        public Attraction(string id, string name, string type, string locale, Externallinks externallinks)
        {
            Id = id;
            Name = name;
            Type = type;
            Locale = locale;
            Externallinks = externallinks;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Type)}: {Type}, " +
                   $" {nameof(Id)}: {Id}, {nameof(Locale)}: {Locale}, {nameof(Externallinks)}: {Externallinks}";
        }

    }

    public class Externallinks
    {
        [BsonElement("Youtube")]
        public Youtube[] Youtube { get; set; }

        [BsonElement("Twitter")]
        public Twitter[] Twitter { get; set; }

        [BsonElement("Itunes")]
        public Itune[] Itunes { get; set; }

        [BsonElement("Lastfm")]
        public Lastfm[] Lastfm { get; set; }

        [BsonElement("Facebook")]
        public Facebook[] Facebook { get; set; }

        [BsonElement("Wiki")]
        public Wiki[] Wiki { get; set; }

        [BsonElement("Instagram")]
        public Instagram[] Instagram { get; set; }

        [BsonElement("Homepage")]
        public Homepage[] Homepage { get; set; }

        public Externallinks(Youtube[] youtube, Twitter[] twitter, Itune[] itunes, Lastfm[] lastfm, Facebook[] facebook, Wiki[] wiki, Instagram[] instagram, Homepage[] homepage)
        {
            Youtube = youtube;
            Twitter = twitter;
            Itunes = itunes;
            Lastfm = lastfm;
            Facebook = facebook;
            Wiki = wiki;
            Instagram = instagram;
            Homepage = homepage;
        }

        public override string ToString()
        {
            return $"{nameof(Youtube)}: {Youtube}, {nameof(Twitter)}: {Twitter}, " +
                   $"{nameof(Itunes)}: {Itunes}, {nameof(Lastfm)}: {Lastfm}, " +
                   $"{nameof(Facebook)}: {Facebook}, {nameof(Wiki)}: {Wiki}, " +
                   $"{nameof(Instagram)}: {Instagram}, {nameof(Homepage)}: {Homepage}";
        }

    }

    public class Youtube
    {
        [BsonElement("Url")]
        public string Url { get; set; }

        public Youtube(string url)
        {
            Url = url;
        }

        public override string ToString()
        {
            return $"{nameof(Url)}: {Url}";
        }

    }

    public class Twitter
    {
        [BsonElement("Url")]
        public string Url { get; set; }

        public Twitter(string url)
        {
            Url = url;
        }

        public override string ToString()
        {
            return $"{nameof(Url)}: {Url}";
        }

    }

    public class Itune
    {
        [BsonElement("Url")]
        public string Url { get; set; }
        public Itune(string url)
        {
            Url = url;
        }

        public override string ToString()
        {
            return $"{nameof(Url)}: {Url}";
        }

    }

    public class Lastfm
    {
        [BsonElement("Url")]
        public string Url { get; set; }

        public Lastfm(string url)
        {
            Url = url;
        }

        public override string ToString()
        {
            return $"{nameof(Url)}: {Url}";
        }

    }

    public class Facebook
    {
        [BsonElement("Url")]
        public string Url { get; set; }

        public Facebook(string url)
        {
            Url = url;
        }

        public override string ToString()
        {
            return $"{nameof(Url)}: {Url}";
        }

    }

    public class Wiki
    {
        [BsonElement("Url")]
        public string Url { get; set; }

        public Wiki(string url)
        {
            Url = url;
        }
        public override string ToString()
        {
            return $"{nameof(Url)}: {Url}";
        }

    }

    public class Instagram
    {
        [BsonElement("Url")]
        public string Url { get; set; }

        public Instagram(string url)
        {
            Url = url;
        }

        public override string ToString()
        {
            return $"{nameof(Url)}: {Url}";
        }

    }

    public class Homepage
    {
        [BsonElement("Url")]
        public string Url { get; set; }

        public Homepage(string url)
        {
            Url = url;
        }

        public override string ToString()
        {
            return $"{nameof(Url)}: {Url}";
        }

    }

    public class Image
    {
        [BsonElement("Url")]
        public string Url { get; set; }

        [BsonElement("Ratio")]
        public string Ratio { get; set; }

        [BsonElement("Width")]
        public int? Width { get; set; }

        [BsonElement("Height")]
        public int? Height { get; set; }

        public Image(string url, string ratio, int? width, int? height)
        {
            Url = url;
            Ratio = ratio;
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"{nameof(Url)}: {Url}, {nameof(Ratio)}: {Ratio}, " +
                   $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}";
        }
    }
}