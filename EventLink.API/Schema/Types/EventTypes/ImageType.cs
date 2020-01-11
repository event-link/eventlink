using EventLink.DataAccess.Models;
using GraphQL.Types;

namespace EventLink.API.Schema.Types.EventTypes
{
    public class ImageType : ObjectGraphType<Image>
    {
        public ImageType()
        {
            Field(x => x.Url).Description("The URL to the image");
            Field(x => x.Ratio).Description("The ratio of the image");
            Field(x => x.Width, nullable: true).Description("The width of the image");
            Field(x => x.Height, nullable: true).Description("The height of the image");
        }
    }
}