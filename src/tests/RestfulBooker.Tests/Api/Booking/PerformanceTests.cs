using FluentAssertions;
using RestfulBooker.Tests.Dtos;
using RestfulBooker.Tests.Utils;
using System.Diagnostics;

namespace RestfulBooker.Tests.Api.Booking;

/// <summary>
/// Performance test(s) for the Booking API.
/// Currently contains a single benchmark that verifies
/// the GET /booking endpoint returns within 2 seconds.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PerformanceTests : BaseApiTest
{
    private readonly ApiClient _client = new();
   [Test]
    public async Task GetAllBookings_ShouldRespondUnder2Seconds()
    {
        // Arrange.
        var stopwatch = Stopwatch.StartNew();
        var acceptableTimeInMilliseconds = 2000;

        // Act.
        var response = await _client.GetAsync<List<BookingDto>>("booking");
        stopwatch.Stop();

        // Assert.
        response.IsSuccessful.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(acceptableTimeInMilliseconds);
    }
}