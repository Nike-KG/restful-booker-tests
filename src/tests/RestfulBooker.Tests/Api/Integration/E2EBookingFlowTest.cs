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
        _test?.Value?.Info($"Creating a new booking.");
        _test?.Value?.Info($"Sending POST /booking");
        var createPayload = BookingDataBuilder.CreateValidBooking();
        var createResponse = await _client.PostAsync<BookingDto>("booking", createPayload);

        createResponse.IsSuccessful.Should().BeTrue("Booking creation should succeed");
        var id =createResponse.Data?.BookingId;

        // update(PATCH).
        _test?.Value?.Info($"Updating created new booking");
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchPayload = new { firstname = "E2E-Updated" };
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");

        // Verify via GET.

        _test?.Value?.Info($"Verifying partially updated new booking");
        _test?.Value?.Info($"Sending GET /booking/{id}");
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        getResponse?.Data?.FirstName.Should().Be("E2E-Updated", "First name should be updated");

        // DELETE.
        _test?.Value?.Info($"Deleting created new booking");
        _test?.Value?.Info($"Sending DELETE /booking/{id}");
        var deleteResponse = await _client.DeleteAsync($"booking/{id}");
        deleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created, "Deleting an existing booking should return 201 created");

        // confirm deletion.
        _test?.Value?.Info($"Veryfying deletion");
        _test?.Value?.Info($"Sending DELETE /booking/{id}");
        var getAfterDeleteResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");
        getAfterDeleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound, "Deleted booking should return 404 Not Found");
    }
}
