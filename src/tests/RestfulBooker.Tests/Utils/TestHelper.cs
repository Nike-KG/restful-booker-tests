using RestfulBooker.Tests.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestfulBooker.Tests.Utils;

public class TestHelper
{
    public static async Task<int> CreateSampleBookingAsync(ApiClient client)
    {
        var payload = BookingDataBuilder.CreateValidBooking();

        var createResponse = await client.PostAsync<BookingDto>("booking", payload);
        return createResponse.Data!.BookingId;
    }

    public static IEnumerable<int> ExtractIds(List<BookinIdDto> bookingIdDtos)
        => bookingIdDtos.Select(b => b.BookingId).ToList();
}
