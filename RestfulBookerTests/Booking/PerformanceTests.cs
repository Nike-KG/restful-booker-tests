using FluentAssertions;
using RestfulBookerTests.Api;
using System.Diagnostics;

namespace RestfulBookerTests.Booking;

public class PerformanceTests
{
    private readonly ApiClient _client = new();
    [Fact]
    public async Task GetAllBookings_ShouldRespondUnder2Seconds()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();
        var acceptableTimeInMilliseconds = 2000;

        // Act
        var response = await _client.GetAsync("booking");
        stopwatch.Stop();
  
        // Assert
        response.IsSuccessful.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(acceptableTimeInMilliseconds);
    }
}
