using System.Text.Json.Serialization;

namespace RestfulBookerTests.Dtos;

/// <summary>
/// Represents the payload for creating or updating a booking.
/// </summary>
public class BookingDto
{
    [JsonPropertyName("firstname")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastname")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("totalprice")]
    public int TotalPrice { get; set; }

    [JsonPropertyName("depositpaid")]
    public bool DepositPaid { get; set; }

    [JsonPropertyName("bookingdates")]
    public BookingDatesDto? BookingDates { get; set; }

    [JsonPropertyName("additionalneeds")]
    public string? AdditionalNeeds { get; set; }
}

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