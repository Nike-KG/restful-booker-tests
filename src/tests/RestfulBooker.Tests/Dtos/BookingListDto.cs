using System.Text.Json.Serialization;

namespace RestfulBooker.Tests.Dtos;

public class BookinIdDto
{
    [JsonPropertyName("bookingid")]
    public int BookingId { get; set; }
}
