using FluentAssertions;
using RestfulBooker.Tests.Utils;
using System.Net;
using System.Text.Json;

namespace RestfulBooker.Tests.Api.Booking;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class DeleteBookingTests : BaseApiTest
{
    private readonly ApiClient _client = new();

    // Reuse the create helper from UpdateBookingTests.
    private async Task<int> CreateSampleBookingAsync()
    {
        var payload = new
        {
            firstname = "Delete",
            Lastname = "User",
            totalprice = 100,
            depositpaid = true,
            bookingdates = new { checkin = "2025-11-01", checkout = "2025-11-15" },
            additionalneeds = "None"
        };

        var createResponse = await _client.PostAsync("booking", payload);
        return JsonDocument.Parse(createResponse.Content!)
            .RootElement
            .GetProperty("bookingid")
            .GetInt32();
    }

    [Test]
    public async Task Delete_ExistingBooking_ShouldRemoveBooking()
    {
        // Arrange
        var id = await CreateSampleBookingAsync();

        // Act
        var deleteResponse = await _client.DeleteAsync($"booking/{id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, "Deleting an existing booking should return 204 No content");

        // Verify the booking is deleted
        var getResponse = await _client.GetByIdAsync($"booking/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "Deleted booking should return 404 Not Found");
    }

    [Test]
    public async Task Delete_NonExistentBookingId_ShouldReturn404()
    {
        // Arrange
        var response = await _client.DeleteAsync("booking/99999");
        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound, "Deleting a non-existent booking should return 404 Not found");
    }

    [Test]
    public async Task Delete_WithoutAuth_ShouldReturn401Or403()
    {
        // Arrange
        var id = await CreateSampleBookingAsync();

        var response = await _client.DeleteAsync($"booking/{id}", isWithCookieHeader: false);
        response.StatusCode.Should()
            .BeOneOf([HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized],
                "Deleting a booking without authentication should return 403 Forbidden or 401 Unauthorized");
    }
}