using FluentAssertions;
using RestfulBooker.Tests.Utils;
using System.Text.Json;

namespace RestfulBooker.Tests.Api.Booking;

public class GetBookingTests
{
    private readonly ApiClient _client = new();
    private int createdBookingId;

    [OneTimeSetUp]
    public async Task Setup()
    {
        // Ensure there is at least one booking to filter against
        createdBookingId = await CreateSampleBookingAsync();
    }

    //filter by first name
    [Test]
    public async Task Booking_GetByFirstName_ShouldReturnMatchingIds()
    {
        // Arrange & Act
        var response = await _client.GetAsync("booking?firstname=Jim");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = ExtractIds(response.Content!);
        ids.Should().HaveCountGreaterThan(0);
    }

    //filter by last name
    [Test]
    public async Task Booking_GetByLastName_ShouldReturnMatchingIds()
    {
        // Arrange & Act
        var response = await _client.GetAsync("booking?lastname=Brown");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = ExtractIds(response.Content!);
        ids.Should().HaveCountGreaterThan(0);
    }

    //filter by checkin date
    [Test]
    public async Task Booking_GetByCheckinDate_ShouldReturnMatchingIds()
    {
        // Arrange & Act
        var response = await _client.GetAsync("booking?checkin=2018-01-01");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = ExtractIds(response.Content!);
        ids.Should().HaveCountGreaterThan(0);
    }

    //filter by checkout date
    [Test]
    public async Task Booking_GetByCheckoutDate_ShouldReturnMatchingIds()
    {
        // Arrange & Act
        var response = await _client.GetAsync("booking?checkout=2019-01-01");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = ExtractIds(response.Content!);
        ids.Should().HaveCountGreaterThan(0);
    }

    //combined filters - need to improve
    [Test]
    public async Task Booking_GetByMultipleFilters_ShouldReturnMatchingIds()
    {
        // Arrange & Act
        var response = await _client.GetAsync("booking?firstname=Get&lastname=User");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = ExtractIds(response.Content!);
        ids.Should().HaveCountGreaterThan(0);
    }

    [Test]
    public async Task Get_BookingIds_ShouldReturnArrayOfIds()
    {
        // Arrange
        var apiClient = new ApiClient();

        // Act
        var response = await apiClient.GetAsync("booking");

        // Assert
        response.IsSuccessful.Should().BeTrue("API should return 200 OK");
        response.Content.Should().NotBeNullOrEmpty();

        // Deserialize Json to dynamic
        var json = JsonDocument.Parse(response.Content!);
        json.RootElement.GetArrayLength().Should().BeGreaterThan(0, "there should be at least one booking in the system");

    }

    [Test]
    public async Task Get_WithoutAuth_ShouldReturn403()
    {
        // Arrange
        var id = createdBookingId;

        var response = await _client.GetAsync($"booking/{id}", isWithCookieHeader: false);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK, "Getting a booking without authentication should return 200 OK");
    }

    private async Task<int> CreateSampleBookingAsync()
    {
        var payload = new
        {
            firstname = "Get",
            Lastname = "User",
            totalprice = 100,
            depositpaid = true,
            bookingdates = new { checkin = "2025-11-02", checkout = "2025-11-16" },
            additionalneeds = "None"
        };

        var createResponse = await _client.PostAsync("booking", payload);
        return JsonDocument.Parse(createResponse.Content!)
            .RootElement
            .GetProperty("bookingid")
            .GetInt32();
    }

    private static IEnumerable<int> ExtractIds(string json)
        => JsonDocument.Parse(json)
        .RootElement.EnumerateArray()
        .Select(e => e.GetProperty("bookingid").GetInt32());
}
