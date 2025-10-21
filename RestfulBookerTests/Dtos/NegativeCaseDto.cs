using System.Text.Json.Serialization;

namespace RestfulBookerTests.Dtos;

public class NegativeCaseDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("payload")]
    public object? Payload { get; set; }

    [JsonPropertyName("expectedStatus")]
    public int ExpectedStatus { get; set; }
}