using FluentAssertions;
using RestfulBookerTests.Api;
using System.Text.Json;

namespace RestfulBookerTests.Booking
{
    public class DeleteBookingTests
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
                bookingdates = new { checkin = "2025-11-01", checkout = "2025-11-15"},
                additionalneeds = "None"
            };

            var createResponse = await _client.PostAsync("booking", payload);
            return JsonDocument.Parse(createResponse.Content!)
                .RootElement
                .GetProperty("bookingid")
                .GetInt32();
        }

        [Fact]
        public async Task Delete_ExistingBooking_ShouldRemoveBooking()
        {
            // Arrange
            var id = await CreateSampleBookingAsync();

            // Act
            var deleteResponse = await _client.DeleteAsync($"booking/{id}");

            // Assert
            deleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created, "Deleting an existing booking should return 201 Created");

            // Verify the booking is deleted
            var getResponse = await _client.GetAsync($"booking/{id}");
            getResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound, "Deleted booking should return 404 Not Found");
        }

        [Fact]
        public async Task Delete_NonExistentBookingId_ShouldReturn404()
        {
            // Arrange
            var response = await _client.DeleteAsync("booking/99999");
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.MethodNotAllowed, "Deleting a non-existent booking should return 405 Method Not Allowed");
        }

        [Fact]
        public async Task Delete_WithoutAuth_ShouldReturn403()
        {
            // Arrange
            var id = await CreateSampleBookingAsync();

            var response = await _client.DeleteAsync($"booking/{id}", isWithCookieHeader: false);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden, "Deleting a booking without authentication should return 403 Forbidden");
        }


    }
}
