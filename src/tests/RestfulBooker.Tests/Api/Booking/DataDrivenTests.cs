using FluentAssertions;
using RestfulBooker.Tests.Dtos;
using RestfulBooker.Tests.Utils;
using System.Text.Json;

namespace RestfulBooker.Tests.Api.Booking;

/// <summary>
/// Integration tests for the **DELETE /booking/{id}** endpoint.
/// 
/// Each test follows a clear pattern:
/// 1. **Arrange** – create (or pick) a booking ID via `TestHelper.CreateSampleBookingAsync`.  
/// 2. **Act** – send a DELETE request to `/booking/{id}` (the client can omit the auth cookie if desired).  
/// 3. **Assert** – verify the HTTP status code and, when appropriate, that a subsequent GET for the same ID returns *404 NotFound* (i.e., the booking has been removed).
/// 
/// The suite covers:
///   • **Deleting an existing booking** – expects *201 Created* and that the booking is subsequently gone.  
///   • **Deleting a non‑existent ID** – expects *404 NotFound*.  
///   • **Deleting without authentication** – expects *401 Unauthorized*.
/// 
/// Tests are marked `[Parallelizable]` so they run concurrently, speeding up the overall test run.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class DataDrivenTests : BaseApiTest
{
    private readonly ApiClient _client = new();

    [Test]
    [TestCaseSource(nameof(LoadBookingData))]
    public async Task CreateBooking_WithVariousPayloads_ShouldSucceed(object payload)
    {
        _test?.Value?.Info("Sending Post /booking");
        // Arrange & Act.
        var response = await _client.PostAsync<BookingDto>("booking", payload);
        response.IsSuccessful.Should().BeTrue("Booking creation should succeed");
        var id = response?.Data?.BookingId;

        // Assert.
        id.Should().BePositive();

        // Cleanup.
        _test?.Value?.Info($"Sending DELETE /booking/{id}");
        await _client.DeleteAsync($"booking/{id}");
    }

    public static IEnumerable<object[]> LoadBookingData()
    => TestDataLoader<BookingDto>.Load("bookingData.json");
}
