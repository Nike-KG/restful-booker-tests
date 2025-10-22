using FluentAssertions;
using RestfulBooker.Tests.Dtos;
using RestfulBooker.Tests.Utils;
using System.Net;

namespace RestfulBooker.Tests.Api.Booking;


/// <summary>
/// API tests for the **GET /booking** endpoint.
///
/// Each test follows a common pattern:
/// 1. Ensure there is at least one booking in the system  
///    (created once in <see cref="OneTimeSetup"/> via `TestHelper.CreateSampleBookingAsync`).  
/// 2. Send a GET request – optionally with query parameters for filtering.  
/// 3. Assert that the HTTP status is successful (200 OK) and that the returned list of
///    booking IDs contains at least one element, or that a 401 Unauthorized is returned
///    when authentication is omitted.
///
/// The suite covers:
///   • `GET /booking` without query parameters – returns an array of all booking IDs.
///   • Filtering by *firstname* or *lastname* individually.  
///   • Filtering by *checkin* date (returns bookings whose check‑in is on or after the given date).  
///   • Filtering by *checkout* date (returns bookings whose check‑out is on or after the given date).  
///   • Combined string filters (`firstname&lastname`).  
///   • Combined date filters (`checkin&checkout`).  
///   • Attempting to retrieve a booking without authentication – expects 401 Unauthorized.
///
/// All tests are marked `[Parallelizable]` so they run concurrently, speeding up the test
/// suite while still sharing a single `ApiClient` instance.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class GetBookingTests : BaseApiTest
{
    private readonly ApiClient _client = new();
    private int createdBookingId;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        // Ensure there is at least one booking to filter against
        createdBookingId = await TestHelper.CreateSampleBookingAsync(_client);
    }

    //filter by first name
    [Test]
    public async Task Booking_GetByFirstName_ShouldReturnMatchingIds()
    {
        _test?.Value?.Info("Sending GET /booking?firstname=Firstname-Test-Kaz");
        // Arrange & Act
        var response = await _client.GetAsync<List<BookinIdDto>>("booking?firstname=Firstname-Test-Kaz");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = TestHelper.ExtractIds(response.Data!);
        ids.Should().HaveCountGreaterThan(0);
    }

    //filter by last name
    [Test]
    public async Task Booking_GetByLastName_ShouldReturnMatchingIds()
    {
        _test?.Value?.Info("Sending GET /booking?lastname=Lastname Test-Kaz");
        // Arrange & Act
        var response = await _client.GetAsync<List<BookinIdDto>>("booking?lastname=Lastname-Test-Kaz");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = TestHelper.ExtractIds(response.Data!);
        ids.Should().HaveCountGreaterThan(0);
    }

    //filter by checkin date
    [Test]
    public async Task Booking_GetByCheckinDate_ShouldReturnAllDatesGreaterThanOrEqual()
    {
        _test?.Value?.Info("Sending GET /booking?checkin=2025-01-01");
        // Arrange & Act
        var response = await _client.GetAsync<List<BookinIdDto>>("booking?checkin=2025-01-01");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = TestHelper.ExtractIds(response.Data!);
        ids.Should().HaveCountGreaterThan(0);
    }

    //filter by checkout date
    [Test]
    public async Task Booking_GetByCheckoutDate_ShouldReturnAllDatesGreaterThanOrEqual()
    {
        _test?.Value?.Info("Sending GET /booking?checkout=2025-01-01");
        // Arrange & Act
        var response = await _client.GetAsync<List<BookinIdDto>>("booking?checkout=2025-01-01");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = TestHelper.ExtractIds(response.Data!);
        ids.Should().HaveCountGreaterThan(0);
    }

    //combined filters - need to improve
    [Test]
    public async Task Booking_GetByMultipleFiltersFirstnameLastname_ShouldReturnMatchingIds()
    {
        _test?.Value?.Info("Sending GET /booking?firstname=Firstname-Test-Kaz&lastname=Lastname-Test-Kaz");
        // Arrange & Act
        var response = await _client.GetAsync<List<BookinIdDto>>("booking?firstname=Firstname-Test-Kaz&lastname=Lastname-Test-Kaz");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = TestHelper.ExtractIds(response.Data!);
        ids.Should().HaveCountGreaterThan(0);
    }

    [Test]
    public async Task Booking_GetByMultipleFiltersDates_ShouldReturnMatchingIds()
    {
        _test?.Value?.Info("Sending GET /booking?checkin=2025-01-01&checkout=2025-12-01");

        // Arrange & Act
        var response = await _client.GetAsync<List<BookinIdDto>>("booking?checkin=2025-01-01&checkout=2025-12-01");

        // Assert
        response.IsSuccessful.Should().BeTrue();
        var ids = TestHelper.ExtractIds(response.Data!);
        ids.Should().HaveCountGreaterThan(0);
    }

    [Test]
    public async Task Get_BookingIds_ShouldReturnArrayOfIds()
    {
        _test?.Value?.Info("Sending GET /booking");
        // Arrange
        var apiClient = new ApiClient();

        // Act
        var response = await apiClient.GetAsync<List<BookinIdDto>>("booking");

        // Assert
        response.IsSuccessful.Should().BeTrue("API should return 200 OK");
        response.Data.Should().NotBeNullOrEmpty();
        var ids = TestHelper.ExtractIds(response.Data!);
        ids.Should().HaveCountGreaterThan(0);

    }

    [Test]
    public async Task Get_WithoutAuth_ShouldReturn401()
    {
        // Arrange
        var id = createdBookingId;
        _test?.Value?.Info($"Sending GET /booking/{id}");

        var response = await _client.GetAsync<BookingDto>($"booking/{id}", isWithCookieHeader: false);
        response.StatusCode.Should().
            Be(HttpStatusCode.Unauthorized,
                "Getting a booking without authentication should return 401 Unauthorized");
    }
}
