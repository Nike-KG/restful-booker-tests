using FluentAssertions;
using RestfulBooker.Tests.Dtos;
using RestfulBooker.Tests.Utils;
using System.Text.Json;

namespace RestfulBooker.Tests.Api.Booking;

public class DataDrivenTests : BaseApiTest
{
    private readonly ApiClient _client = new();

    // Create booking using data from JSON.
    [Test]
    [TestCaseSource(nameof(LoadBookingData))]
    public async Task CreateBooking_WithVariousPayloads_ShouldSucceed(object payload)
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
    => TestDataLoader<BookingDto>.Load("bookingData.json");
}
