using FluentAssertions;
using RestfulBooker.Tests.Dtos;
using RestfulBooker.Tests.Utils;
using System.Net;
using System.Text.Json;

namespace RestfulBooker.Tests.Api.Booking;

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
        _test?.Value?.Info("Sending GET /booking?checking=2018-01-01&checkout=2025-12-01");

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
        response.Content.Should().NotBeNullOrEmpty();

        // Deserialize Json to dynamic
        var json = JsonDocument.Parse(response.Content!);
        json.RootElement.GetArrayLength().Should().BeGreaterThan(0, "there should be at least one booking in the system");

    }

    [Test]
    public async Task Get_WithoutAuth_ShouldReturn401Or403()
    {
        // Arrange
        var id = createdBookingId;
        _test?.Value?.Info($"Sending GET /booking/{id}");

        var response = await _client.GetAsync<BookingDto>($"booking/{id}", isWithCookieHeader: false);
        response.StatusCode.Should().
            BeOneOf([HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized],
                "Getting a booking without authentication should return 401 Unauthorized or 403 Forbidden");
    }
}
