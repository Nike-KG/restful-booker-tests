using FluentAssertions;
using RestfulBookerTests.Api;

namespace RestfulBookerTests.Booking;

public class GetBookingIdsTest
{
   [Fact]
   public async Task GetBookingIds_ShouldReturnArrayOfIds()
    {
         // Arrange
         var apiClient = new ApiClient();
    
         // Act
         var response =  await apiClient.GetAsync("booking");
    
         // Assert
         response.IsSuccessful.Should().BeTrue("API should return 200 OK");
         response.Content.Should().NotBeNullOrEmpty();

        // Deserialize Json to dynamic
        var json = System.Text.Json.JsonDocument.Parse(response.Content!);
        json.RootElement.GetArrayLength().Should().BeGreaterThan(0, "there should be at least one booking in the system");

    }
}
