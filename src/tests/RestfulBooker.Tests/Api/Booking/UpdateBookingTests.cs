using FluentAssertions;
using RestfulBooker.Tests.Dtos;
using RestfulBooker.Tests.Utils;
using System.Net;

namespace RestfulBooker.Tests.Api.Booking;

/// <summary>
/// API tests for the PATCH /booking/{id} endpoint.
///
/// Each test follows the pattern:
///   1. Create a sample booking (via TestHelper).
///   2. Send a PATCH request with a specific payload.
///   3. Assert the HTTP status and that a subsequent GET returns
///      the expected data.
///
/// The suite covers:
///   • Single‑field updates (firstname, lastname, totalprice, depositpaid)
///   • Multi‑field update
///   • Empty body (should succeed – 200 OK)
///   • Invalid data types for each field (400 Bad Request)
///   • Malformed bookingdates property (400 Bad Request)
///   • Non‑existent ID (404 Not Found)
///   • Missing authentication header (401 Unauthorized)
///
/// Tests run in parallel to speed up the suite.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class UpdateBookingTests : BaseApiTest
{
    private readonly ApiClient _client = new();

   [Test]
    public async Task Patch_SingleFlield_ShouldUpdateFirstName()
    {
        // Arrange.
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);
        var patchPayload = new { firstname = "John" };

        // Act.
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        _test?.Value?.Info($"Getting partially updated booking details.");
        _test?.Value?.Info($"Sending GET /booking/{id}");
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");


        // Assert.
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        getResponse?.Data?.FirstName.Should().Be("John", "First name should be updated"); ;
    }

   [Test]
    public async Task Patch_SingleField_ShouldUpdateLastName()
    {
        // Arrange.
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);
        var patchPayload = new { lastname = "Petty" };

        // Act.
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        _test?.Value?.Info($"Getting partially updated booking details.");
        _test?.Value?.Info($"Sending GET /booking/{id}");
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");

        // Assert.
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        getResponse?.Data?.LastName.Should().Be("Petty", "Last name should be updated"); 
    }

   [Test]
    public async Task Patch_SingleField_ShouldUpdateTotalPrice()
    {
        // Arrange.
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);
        var patchPayload = new { totalprice = 250 };

        // Act.
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        _test?.Value?.Info($"Getting partially updated booking details.");
        _test?.Value?.Info($"Sending GET /booking/{id}");
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");

        // Assert.
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        getResponse?.Data?.TotalPrice.Should().Be(250, "Last name should be updated");
    }

   [Test]
    public async Task Patch_SingleField_ShouldUpdateDepositPaid()
    {
        // Arrange.
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);
        var patchPayload = new { depositpaid = false };

        // Act.
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        _test?.Value?.Info($"Getting partially updated booking details.");
        _test?.Value?.Info($"Sending GET /booking/{id}");
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");

        // Assert.
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        getResponse?.Data?.DepositPaid.Should().BeFalse("Deposit paid should be updated");
    }

   [Test]
    public async Task Patch_MultipleFields_ShouldUpdateAllSpecified()
    {
        // Arrange.
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);
        var patchPayload = BookingDataBuilder.PatchBookingMultipleFields(
            "Multi", "Update", 300, false, "2025-02-01", "2025-10-01", "Late Checkout");

        // Act.
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        _test?.Value?.Info($"Getting partially updated booking details.");
        _test?.Value?.Info($"Sending GET /booking/{id}");
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");

        // Assert.
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");

        getResponse?.Data?.FirstName.Should().Be("Multi", "First name should be updated");
        getResponse?.Data?.LastName.Should().Be("Update", "Last name should be updated");
        getResponse?.Data?.TotalPrice.Should().Be(300, "Total price should be updated");
        getResponse?.Data?.DepositPaid.Should().BeFalse("Deposit paid should be updated");
        getResponse?.Data?.BookingDates?.CheckIn.Should().Be("2025-02-01", "Checkin should be updated");
        getResponse?.Data?.BookingDates?.CheckOut.Should().Be("2025-10-01", "Checkout name should be updated");
        getResponse?.Data?.AdditionalNeeds.Should().Be("Late Checkout", "Additional needs should be updated");
    }

    [Test]
    public async Task Patch_EmptyBody_ShouldReturnBadRequest()
    {
        // Arrange.
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);
        var patchPayload = new { };

        // Act.
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        // Assert.
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");
        patchResponse.StatusCode.Should().
                   Be(HttpStatusCode.OK, "Patching non-existent booking should return 200 Not found");
    }

    [Test]
    public async Task Patch_InvalidFirstnameDataType_ShouldReturnBadRequest()
    {
        // Arrange.
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);
        var patchPayload = new { firstname = 124 };

        // Act.
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        // Assert.
        patchResponse.IsSuccessful.Should().BeFalse("Booking patch should succeed");
        patchResponse.StatusCode.Should().
                   Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Patch_InvalidLastnameDataType_ShouldReturnBadRequest()
    {
        // Arrange.
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);
        var patchPayload = new { lastname = 124 };

        // Act.
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        // Assert.
        patchResponse.IsSuccessful.Should().BeFalse("Booking patch should succeed");
        patchResponse.StatusCode.Should().
                   Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Patch_InvalidTotalPriceDataType_ShouldReturnBadRequest()
    {
        // Arrange.
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);
        var patchPayload = new { totalprice = "price" };

        // Act.
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        // Assert.
        patchResponse.IsSuccessful.Should().BeFalse("Booking patch should succeed");
        patchResponse.StatusCode.Should().
                   Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Patch_InvalidDepositPaidDataType_ShouldReturnBadRequest()
    {
        // Arrange.
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);
        var patchPayload = new { depositpaid = "price" };

        // Act.
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        // Assert.
        patchResponse.IsSuccessful.Should().BeFalse("Booking patch should succeed");
        patchResponse.StatusCode.Should().
                   Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Patch_InvalidChckinData_ShouldReturnBadRequest()
    {
        // Arrange.
        _test?.Value?.Info($"Creating sample booking");
        var id = await TestHelper.CreateSampleBookingAsync(_client);
        var patchPayload = new { bookingdates = new { chckin = "checkin"} };

        // Act.
        _test?.Value?.Info($"Sending PATCH /booking/{id}");
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);

        // Assert.
        patchResponse.IsSuccessful.Should().BeFalse("Booking patch should succeed");
        patchResponse.StatusCode.Should().
                   Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Patch_NonExistentId_ShouldReturn404()
    {
        // Arrange.
        var patchPayload = new { firstname = "Ghost" };

        // Act.
        _test?.Value?.Info("Sending PATCH /booking/99999");
        var response = await _client.PatchAsync<BookingDto>("booking/99999", patchPayload);

        // Assert.
        response.StatusCode.Should().
            Be(HttpStatusCode.NotFound, "Patching non-existent booking should return 404 Not found");
    }

   [Test]
    public async Task Patch_WithoutAuth_ShouldReturn401()
    {
        // Arrange.
        var patchPayload = new { firstname = "NoAuth" };

        // Act.
        _test?.Value?.Info("Sending PATCH /booking/1");
        var response = await _client.PatchAsync<BookingDto>("booking/1", patchPayload, false);

        // Assert.
        response.StatusCode.Should().
            Be(HttpStatusCode.Unauthorized,
                "Patching without auth should return 401 Unauthorized");
    }
}
