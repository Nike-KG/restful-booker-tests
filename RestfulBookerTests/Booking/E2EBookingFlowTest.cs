using FluentAssertions;
using RestfulBookerTests.Api;
using System.Text.Json;

namespace RestfulBookerTests.Booking;

public class E2EBookingFlowTest
{
    private readonly ApiClient _client = new();

    [Fact]
    public async Task FullBookingLifecycle_ShouldSucceed()
    {
        // Create.
        var createPayload = new
        {
            firstname = "E2E",
            lastname = "Flow",
            totalprice = 500,
            depositpaid = true,
            bookingdates = new
            {
                checkin = "2025-12-02",
                checkout = "2025-12-12"
            },
            additionalneeds = "Dinner"
        };

        var createResponse = await _client.PostAsync("booking", createPayload);
        createResponse.IsSuccessful.Should().BeTrue("Booking creation should succeed");
        var id = JsonDocument.Parse(createResponse.Content!)
            .RootElement
            .GetProperty("bookingid")
            .GetInt32();

        // update(PATCH).
        var patchPayload = new { firstname = "E2E-Updated" };
        var patchResponse = await _client.PatchAsync($"booking/{id}", patchPayload);
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");

        // Verify via GET.
        var getResponse = await _client.GetAsync($"booking/{id}");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        var json = JsonDocument.Parse(getResponse.Content!);
        json.RootElement.GetProperty("firstname").GetString().Should().Be("E2E-Updated", "First name should be updated");

        // DELETE.
        var deleteResponse = await _client.DeleteAsync($"booking/{id}");
        deleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created, "Deleting an existing booking should return 201 created");

        // confirm deletion.
        var getAfterDeleteResponse = await _client.GetAsync($"booking/{id}");
        getAfterDeleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound, "Deleted booking should return 404 Not Found");
    }






}
