using System.Text.Json.Serialization;

namespace RestfulBooker.Tests.Dtos;

/// <summary>
/// Nested dates object inside a booking.
/// </summary>
public class BookingDatesDto
{
    [JsonPropertyName("checkin")]
    public string CheckIn { get; set; } = string.Empty;

    [JsonPropertyName("checkout")]
    public string CheckOut { get; set; } = string.Empty;
}