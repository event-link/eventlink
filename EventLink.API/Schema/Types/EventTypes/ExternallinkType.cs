using EventLink.API.Schema.Types.EventTypes.ExternalLinks;
using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class ExternallinkType : ObjectGraphType<Externallinks>
    {
        public ExternallinkType()
        {
            Field<ListGraphType<YoutubeType>>("Youtube");
            Field<ListGraphType<TwitterType>>("Twitter");
            Field<ListGraphType<ItunesType>>("Itunes");
            Field<ListGraphType<LastfmType>>("Lastfm");
            Field<ListGraphType<FacebookType>>("Facebook");
            Field<ListGraphType<WikiType>>("Wiki");
            Field<ListGraphType<InstagramType>>("Instagram");
            Field<ListGraphType<HomepageType>>("Homepage");
        }
    }
}