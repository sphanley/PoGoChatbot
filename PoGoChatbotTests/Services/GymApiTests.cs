using FluentAssertions;
using System.Collections.Generic;
using PoGoChatbot.Models;
using PoGoChatbot.Services;
using Xunit;

namespace PoGoChatbotTests
{
    public class GymApiTests
    {
        private static readonly Gym FREE_LITTLE_LIBRARY = new Gym
        {
            Name = "Free Little Library",
            Aliases = new List<string> { "FLL" },
            Territory = new List<string> { "Near East Side" },
            Location = new LatLng { Latitude = "41.495958", Longitude = "-81.5820074" }
        };

        private static readonly Gym HEIGHTS_LIBRARY = new Gym
        {
            Name = "Cleveland Heights-University Heights Public Library",
            Territory = new List<string> { "Near East Side" },
            Location = new LatLng { Latitude = "41.4957825", Longitude = "-81.5649459" }
        };

        private static readonly Gym SCHOOLHOUSE = new Gym
        {
            Name = "Cleveland Heights Historical Schoolhouse",
            Territory = new List<string> { "Near East Side" },
            Location = new LatLng { Latitude = "41.5094061", Longitude = "-81.5688236" }
        };

        [Fact]
        public void GetGyms_Should_ReturnNoMatches()
        {
            var results = GymApi.GetGyms("A string which does not match any gym name", "Near East Side");

            results.Should().BeEmpty("because the search term should not match any gym name");
        }

        [Theory]
        [InlineData("Free Little Library", "Near East Side")]
        [InlineData("free little library", "Near East Side")]
        [InlineData("free library", "Near East Side")]
        [InlineData("free little libary ", "Near East Side")]
        [InlineData("Free Little Library", "University Circle")]
        [InlineData("FLL", "Near East Side")]
        public void GetGyms_Should_ReturnSingleMatch(string searchTerm, string groupName)
        {
            var results = GymApi.GetGyms(searchTerm, groupName);

            results.Should()
                .NotBeNullOrEmpty()
                .And
                .SatisfyRespectively(result => result.Should().BeEquivalentTo(FREE_LITTLE_LIBRARY));
        }

        [Theory]
        [InlineData("Cleveland Heights", "Near East Side")]
        public void GetGyms_ShouldReturnMultipleMatches(string searchTerm, string groupName)
        {
            var results = GymApi.GetGyms(searchTerm, groupName);
            results.Should()
                .NotBeNullOrEmpty()
                .And
                .SatisfyRespectively(
                    resultA => resultA.Should().BeEquivalentTo(HEIGHTS_LIBRARY),
                    resultB => resultB.Should().BeEquivalentTo(SCHOOLHOUSE)
            );
        }
    }
}
