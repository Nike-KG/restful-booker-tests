using FluentAssertions;
using RestfulBooker.Tests.Dtos;
using RestfulBooker.Tests.Utils;
using System.Net;

namespace RestfulBooker.Tests.Api.Booking;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class DeleteBookingTests : BaseApiTest
{
    private readonly ApiClient _client = new();

    [Test]
    public async Task Delete_ExistingBooking_ShouldRemoveBooking()
    {
        // Arrange
        var id = await TestHelper.CreateSampleBookingAsync(_client);

        // Act
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
        var response = await _client.DeleteAsync("booking/99999");
        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound, "Deleting a non-existent booking should return 404 Not found");
    }

    [Test]
    public async Task Delete_WithoutAuth_ShouldReturn401()
    {
        // Arrange
        var id = await TestHelper.CreateSampleBookingAsync(_client);

        var response = await _client.DeleteAsync($"booking/{id}", isWithCookieHeader: false);
        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized,
                "Deleting a booking without authentication should return 401 Unauthorized");
    }
}