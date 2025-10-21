using FluentAssertions;
using RestfulBookerTests.Api;

namespace RestfulBookerTests.Booking;

public class GetBookingFilterTests
{
    private readonly ApiClient _client = new();

    //filter by first name
    [Fact]
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
    [Fact]
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
    [Fact]
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
    [Fact]
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
    [Fact]
    public async Task Booking_GetByMultipleFilters_ShouldReturnMatchingIds()
    {
        // Arrange & Act
        var response = await _client.GetAsync("booking?firstname=Multi&lastname=Update");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = ExtractIds(response.Content!);
        ids.Should().HaveCountGreaterThan(0);
    }

    private static IEnumerable<int> ExtractIds(string json)
   => System.Text.Json.JsonDocument.Parse(json)
        .RootElement.EnumerateArray()
        .Select(e => e.GetProperty("bookingid").GetInt32());
}
