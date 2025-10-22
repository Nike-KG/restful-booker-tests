using FluentAssertions;
using RestfulBooker.Tests.Utils;
using System.Diagnostics;

namespace RestfulBooker.Tests.Api.Booking;

public class PerformanceTests : BaseApiTest
{
    private readonly ApiClient _client = new();
   [Test]
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