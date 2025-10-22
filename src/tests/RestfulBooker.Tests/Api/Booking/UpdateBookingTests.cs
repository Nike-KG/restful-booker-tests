using FluentAssertions;
using RestfulBooker.Tests.Dtos;
using RestfulBooker.Tests.Utils;
using System.Net;
using System.Text.Json;

namespace RestfulBooker.Tests.Api.Booking;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class UpdateBookingTests : BaseApiTest
{
    private readonly ApiClient _client = new();

   [Test]
    public async Task Patch_SingleFlield_ShouldUpdateFirstName()
    {
        // Arrange
        var id = await TestHelper.CreateSampleBookingAsync(_client);

        var patchPayload = new { firstname = "John" };
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");


        // verify via GET.
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        getResponse?.Data?.FirstName.Should().Be("John", "First name should be updated"); ;
    }

   [Test]
    public async Task Patch_SingleField_ShouldUpdateLastName()
    {
        // Arrange
        var id = await TestHelper.CreateSampleBookingAsync(_client);

        var patchPayload = new { lastname = "Petty" };
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");

        // verify via GET.
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        getResponse?.Data?.LastName.Should().Be("Petty", "Last name should be updated"); 
    }

   [Test]
    public async Task Patch_SingleField_ShouldUpdateTotalPrice()
    {
        // Arrange
        var id = await TestHelper.CreateSampleBookingAsync(_client);

        var patchPayload = new { totalprice = 250 };
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");

        // verify via GET.
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        getResponse?.Data?.TotalPrice.Should().Be(250, "Last name should be updated");
    }

   [Test]
    public async Task Patch_SingleField_ShouldUpdateDepositPaid()
    {
        // Arrange
        var id = await TestHelper.CreateSampleBookingAsync(_client);

        var patchPayload = new { depositpaid = false };
        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");

        // verify via GET.
        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");
        getResponse.IsSuccessful.Should().BeTrue("Booking retrieval should succeed");
        getResponse?.Data?.DepositPaid.Should().BeFalse("Deposit paid should be updated");
    }

   [Test]
    public async Task Patch_MultipleFields_ShouldUpdateAllSpecified()
    {
        // Arrange
        var id = await TestHelper.CreateSampleBookingAsync(_client);

        var patchPayload = BookingDataBuilder.PatchBookingMultipleFields(
            "Multi", "Update", 300, false, "2025-02-01", "2025-10-01", "Late Checkout");


        var patchResponse = await _client.PatchAsync<BookingDto>($"booking/{id}", patchPayload);
        patchResponse.IsSuccessful.Should().BeTrue("Booking patch should succeed");

        var getResponse = await _client.GetByIdAsync<BookingDto>($"booking/{id}");
        getResponse?.Data?.FirstName.Should().Be("Multi", "First name should be updated");
        getResponse?.Data?.LastName.Should().Be("Update", "Last name should be updated");
        getResponse?.Data?.TotalPrice.Should().Be(300, "Total price should be updated");
        getResponse?.Data?.DepositPaid.Should().BeFalse("Deposit paid should be updated");
        getResponse?.Data?.BookingDates?.CheckIn.Should().Be("2025-02-01", "Checkin should be updated");
        getResponse?.Data?.BookingDates?.CheckOut.Should().Be("2025-10-01", "Checkout name should be updated");
        getResponse?.Data?.AdditionalNeeds.Should().Be("Late Checkout", "Additional needs should be updated");
    }

   [Test]
    public async Task Patch_NonExistentId_ShouldReturn404()
    {
        var patchPayload = new { firstname = "Ghost" };
        var response = await _client.PatchAsync<BookingDto>("booking/99999", patchPayload);
        response.StatusCode.Should().
            Be(HttpStatusCode.NotFound, "Patching non-existent booking should return 404 Not found");
    }

   [Test]
    public async Task Patch_WithoutAuth_ShouldReturn401Or403()
    {
        var patchPayload = new { firstname = "NoAuth" };
        var response = await _client.PatchAsync<BookingDto>("booking/1", patchPayload, false);
        response.StatusCode.Should().
            BeOneOf([HttpStatusCode.Forbidden,HttpStatusCode.Unauthorized],
                "Patching without auth should return 403 Forbidden or 401 Unauthorized");
    }
}
