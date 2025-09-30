using Compago.Data;
using Compago.Domain;

namespace Compago.Test.Helper.Domain
{
    public static class TagHelper
    {
        public static TagDTO New(
            short? id = 1,
            string name = "TestTag",
            string? color = "#000000")
        {
            return new TagDTO()
            {
                Id = id,
                Name = name,
                Color = color
            };
        }

        public static Tag NewDb(
            short id = 1,
            string name = "TestTag",
            string color = "#000000")
        {
            return new Tag()
            {
                Id = id,
                Name = name,
                Color = color
            };
        }
    }
}
