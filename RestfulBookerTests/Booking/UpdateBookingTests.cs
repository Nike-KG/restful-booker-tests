using FluentAssertions;
using RestfulBookerTests.Api;
using System.Text.Json;

namespace RestfulBookerTests.Booking;
public class UpdateBookingTests
{
    private readonly ApiClient _client = new();

    // Helper to create a booking that we can update.
    private async Task<int> CreateBookingAsync()
    {
        var payload = new
        {
            firstname = "Update",
            lastname = "Brown",
            totalprice = 111,
            depositpaid = true,
            bookingdates = new
            {
                checkin = "2018-01-01",
                checkout = "2019-01-01"
            },
            additionalneeds = "Breakfast"
        };
        var createResponse = await _client.PostAsync("booking", payload);
        createResponse.IsSuccessful.Should().BeTrue("Booking creation should succeed");
        var id = JsonDocument.Parse(createResponse.Content!)
            .RootElement
            .GetProperty("bookingid")
            .GetInt32();
        return id;
    }

    [Fact]
    public async Task Patch_SingleFlield_ShouldUpdateFirstName()
    {
        // Arrange
        var id = await CreateBookingAsync();

        var patchPayload = new { firstname = "John" };
        var patchResponse = await _client.PatchAsync($"booking/{id}", patchPayload);
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");


        // verify via GET.
        var getResponse = await _client.GetAsync($"booking/{id}");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        var json = JsonDocument.Parse(getResponse.Content!);
        json.RootElement.GetProperty("firstname").GetString().Should().Be("John", "First name should be updated");
    }

    [Fact]
    public async Task Patch_MultipleFields_ShouldUpdateAllSpecified()
    {
        // Arrange
        var id = await CreateBookingAsync();

        var patchPayload = new
        {
            firstname = "Multi",
            lastname = "Update",
            totalprice = 300,
            depositpaid = false,
            bookingdates = new
            {
                checkin = "2025-02-01",
                checkout = "2020-02-10"
            },
            additionalneeds = "Late Checkout"
        };

        var patchResponse = await _client.PatchAsync($"booking/{id}", patchPayload);
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");

        var getResponse = await _client.GetAsync($"booking/{id}");
        var json = JsonDocument.Parse(getResponse.Content!);
        json.RootElement.GetProperty("firstname").GetString().Should().Be("Multi", "First name should be updated");
        json.RootElement.GetProperty("lastname").GetString().Should().Be("Update", "Last name should be updated");

     }

    [Fact]
    public async Task Patch_NonExistentId_ShouldReturn404() 
    {
        var patchPayload = new { firstname = "Ghost" };
        var response = await _client.PatchAsync("booking/99999", patchPayload);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.MethodNotAllowed, "Patching non-existent booking should return 405");
    }

    [Fact]
    public async Task Patch_WithoutAuth_ShouldReturn403()
    {
        var patchPayload = new { firstname = "NoAuth" };
        var response = await _client.PatchAsync("booking/1", patchPayload, false);
        response.StatusCode. Should().Be(System.Net.HttpStatusCode.Forbidden, "Patching without auth should return 403");
    }

}
