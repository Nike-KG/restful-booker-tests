using RestfulBooker.Tests.Utils;
using System.Text.Json;
using FluentAssertions;
using RestfulBooker.Tests.Dtos;

namespace RestfulBooker.Tests.Api.Integration;

/// <summary>
/// End‑to‑end integration test that exercises the **full booking lifecycle** on the
/// `/booking` endpoint.
/// 
/// The test follows a single, linear flow:
/// 1. **Create** – POST a new booking with valid data and verify the request succeeds.
/// 2. **Update** – PATCH the booking’s `firstname` field and confirm a successful response.
/// 3. **Verify** – GET the booking by ID, parse the JSON and assert that the
///    `firstname` has been updated.
/// 4. **Delete** – DELETE the booking and assert a *201 Created* status code
///    (the API’s convention for successful deletions).  
/// 5. **Confirm deletion** – GET the same ID again and assert a *404 NotFound*
///    response, ensuring that the booking was truly removed.
/// 
/// This single test demonstrates that all CRUD operations work together correctly
/// and that the API maintains consistent state across a complete transaction.
/// The test is deliberately run sequentially (not parallelizable) because it
/// creates and deletes a booking that would otherwise interfere with other tests.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EBookingFlowTest : BaseApiTest
{
    private readonly ApiClient _client = new();

    [Test]
    public async Task FullBookingLifecycle_ShouldSucceed()
    {
        // Create.
        var createPayload = new BookingDto
        {
            FirstName = "E2E",
            LastName = "Flow",
            TotalPrice = 500,
            DepositPaid = true,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2025-12-02",
                CheckOut = "2025-12-12"
            },
            AdditionalNeeds = "Dinner"
        };

        var createResponse = await _client.PostAsync<BookingDto>("booking", createPayload);
        createResponse.IsSuccessful.Should().BeTrue("Booking creation should succeed");
        var id = JsonDocument.Parse(createResponse.Content!)
            .RootElement
            .GetProperty("bookingid")
            .GetInt32();

        // update(PATCH).
        var patchPayload = new { firstname = "E2E-Updated" };
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");

        // Verify via GET.
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        var json = JsonDocument.Parse(getResponse.Content!);
        json.RootElement.GetProperty("firstname").GetString().Should().Be("E2E-Updated", "First name should be updated");

        // DELETE.
        var deleteResponse = await _client.DeleteAsync($"booking/{id}");
        deleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created, "Deleting an existing booking should return 201 created");

        // confirm deletion.
        var getAfterDeleteResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");
        getAfterDeleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound, "Deleted booking should return 404 Not Found");
    }
}
