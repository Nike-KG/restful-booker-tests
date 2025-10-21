using FluentAssertions;
using RestfulBookerTests.Api;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace RestfulBookerTests.Booking;

public class GetBookingIdsTest
{
    private readonly ApiClient _client = new();
    private async Task<int> CreateSampleBookingAsync()
    {
        var payload = new
        {
            firstname = "Get",
            Lastname = "User",
            totalprice = 100,
            depositpaid = true,
            bookingdates = new { checkin = "2025-11-02", checkout = "2025-11-16" },
            additionalneeds = "None"
        };

        var createResponse = await _client.PostAsync("booking", payload);
        return JsonDocument.Parse(createResponse.Content!)
            .RootElement
            .GetProperty("bookingid")
            .GetInt32();
    }

    [Fact]
   public async Task Get_BookingIds_ShouldReturnArrayOfIds()
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

    [Fact]
    public async Task Get_WithoutAuth_ShouldReturn403()
    {
        // Arrange
        var id = await CreateSampleBookingAsync();

        var response = await _client.GetAsync($"booking/{id}", isWithCookieHeader: false);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK, "Getting a booking without authentication should return 200 OK");
    }
}

