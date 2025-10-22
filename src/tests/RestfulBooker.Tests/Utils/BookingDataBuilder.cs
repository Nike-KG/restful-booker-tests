using RestfulBooker.Tests.Dtos;

namespace RestfulBooker.Tests.Utils;

public class BookingDataBuilder
{
    // Returns a fully valid booking DTO for happy-path tests
    public static BookingDto CreateValidBooking()
    {
        return new BookingDto
        {
            FirstName = "Firstname-Test-Kaz",
            LastName = "Lastname-Test-Kaz",
            TotalPrice = 120,
            DepositPaid = true,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2025-01-01",
                CheckOut = "2025-12-01"
            },
            AdditionalNeeds = "Breakfast"
        };
    }

    // Returns a booking with custom parameters — useful for variations
    public static BookingDto CreateCustomBooking(
        string firstname,
        string lastname,
        int totalPrice,
        bool depositPaid,
        string? additionalNeeds = null)
    {
        return new BookingDto
        {
            FirstName = firstname,
            LastName = lastname,
            TotalPrice = totalPrice,
            DepositPaid = depositPaid,
            BookingDates = new BookingDatesDto
            {
                CheckIn = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd"),
                CheckOut = DateTime.UtcNow.AddDays(4).ToString("yyyy-MM-dd")
            },
            AdditionalNeeds = additionalNeeds ?? "None"
        };
    }

    // Invalid booking for negative tests
    public static BookingDto CreateInvalidBooking_MissingName()
    {
        return new BookingDto
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            TotalPrice = 150,
            DepositPaid = false,
            BookingDates = new BookingDatesDto
            {
                CheckIn = "2023-01-01",
                CheckOut = "2023-01-02"
            },
            AdditionalNeeds = "Lunch"
        };
    }

    public static CreateBookingDto PatchBookingMultipleFields(
        string firstname,
        string lastname,
        int totalPrice,
        bool depositPaid,
        string checkIn,
        string checkOut,
        string? additionalNeeds = null)
    {
        return new CreateBookingDto
        {
            FirstName = firstname,
            LastName = lastname,
            TotalPrice = totalPrice,
            DepositPaid = depositPaid,
            BookingDates = new BookingDatesDto
            {
                CheckIn = checkIn,
                CheckOut = checkOut
            },
            AdditionalNeeds = additionalNeeds ?? "None"
        };
    }
}
