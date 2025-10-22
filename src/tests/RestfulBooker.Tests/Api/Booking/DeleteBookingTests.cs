using FluentAssertions;
using RestfulBooker.Tests.Dtos;
using RestfulBooker.Tests.Utils;
using System.Net;

namespace RestfulBooker.Tests.Api.Booking;


/// <summary>
/// API tests for the **DELETE /booking/{id}** endpoint.
/// 
/// Each test follows a simple pattern:
/// 1. **Arrange** – create (or pick) a booking ID using `TestHelper.CreateSampleBookingAsync`.  
/// 2. **Act** – send a DELETE request to `/booking/{id}` (the client can omit the auth cookie if desired).  
/// 3. **Assert** – verify the returned HTTP status and, where applicable, that a subsequent GET for the same ID
///    returns *404 NotFound* (i.e., the booking has been removed).
/// 
/// The suite covers:
///   • **Deleting an existing booking** – expects *201 Created* and that a following GET yields 404.  
///   • **Deleting a non‑existent ID** – expects *404 NotFound*.  
///   • **Deleting without authentication** – expects *401 Unauthorized*.
/// 
/// All tests are marked `[Parallelizable]` so they run concurrently, speeding up the test suite.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class DeleteBookingTests : BaseApiTest
{
    private readonly ApiClient _client = new();

    [Test]
    public async Task Delete_ExistingBooking_ShouldRemoveBooking()
    {
        // Arrange
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);

        // Act
        _test?.Value?.Info($"Sending DELETE /booking/{id}");
        var deleteResponse = await _client.DeleteAsync($"booking/{id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Created, "Deleting an existing booking should return 201 Created");

        // Verify the booking is deleted
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "Deleted booking should return 404 Not Found");
    }

    [Test]
    public async Task Delete_NonExistentBookingId_ShouldReturn404()
    {
        // Arrange
        _test?.Value?.Info("Sending DELETE /booking/9999");
        var response = await _client.DeleteAsync("booking/99999");
        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound, "Deleting a non-existent booking should return 404 Not found");
    }

    [Test]
    public async Task Delete_WithoutAuth_ShouldReturn401()
    {
        // Arrange
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);

        // Act.
        _test?.Value?.Info($"Sending DELETE /booking/{id}");
        var response = await _client.DeleteAsync($"booking/{id}", isWithCookieHeader: false);

        // Assert.
        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized,
                "Deleting a booking without authentication should return 401 Unauthorized");
    }
}