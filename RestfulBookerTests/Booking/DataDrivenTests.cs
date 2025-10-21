using FluentAssertions;
using RestfulBookerTests.Api;
using RestfulBookerTests.Dtos;
using System.Text.Json;

namespace RestfulBookerTests.Booking;

public class DataDrivenTests
{
    private readonly ApiClient _client = new();

    // Create booking using data from JSON.
    [Theory]
    [MemberData(nameof(LoadBookingData))]
    public async Task CreateBooking_WithVariousPayloads_ShouldSucceed(object  payload)
    { 
        var response = await _client.PostAsync("booking", payload);
        response.IsSuccessful.Should().BeTrue("Booking creation should succeed");
        var id = JsonDocument.Parse(response.Content!)
            .RootElement
            .GetProperty("bookingid")
            .GetInt32();
        id.Should().BePositive();

        // Cleanup.
        await _client.DeleteAsync($"booking/{id}");
    }

    public static IEnumerable<object[]> LoadBookingData()
    => Utilities.TestDataLoader<BookingDto>.Load("bookingData.json");
}
