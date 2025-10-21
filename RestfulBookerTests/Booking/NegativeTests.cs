using FluentAssertions;
using RestfulBookerTests.Api;
using RestfulBookerTests.Dtos;

namespace RestfulBookerTests.Booking;

public class NegativeTests
{
    private readonly ApiClient _client = new();

    [Theory]
    [MemberData(nameof(LoadNegativeCases))]
    public async Task Patch_WithInvalidPayload_ShouldReturnExpectedError(NegativeCaseDto test)
    {
        /*var response = await _client.PatchAsync($"booking/{test.Id}", test.Payload);
        ((int)response.StatusCode).Should().Be(test.ExpectedStatus);
*/
    }

    public static IEnumerable<object[]> LoadNegativeCases() 
        => Utilities.TestDataLoader<NegativeCaseDto>.Load("negativeCases.json");

}
