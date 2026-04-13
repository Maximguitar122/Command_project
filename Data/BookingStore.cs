using Luftreise_Command_project_.Models;

namespace Luftreise_Command_project_.Data
{
    public static class BookingStore
    {
        private static readonly List<BookingTicket> bookings = new();

        public static void AddBooking(BookingTicket booking)
        {
            booking.Id = bookings.Count == 0 ? 1 : bookings.Max(x => x.Id) + 1;
            bookings.Add(booking);
        }

        public static List<BookingTicket> GetUserBookings(string email)
        {
            return bookings
                .Where(x => x.UserEmail.ToLower() == email.ToLower())
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public static BookingTicket? GetById(int id)
        {
            return bookings.FirstOrDefault(x => x.Id == id);
        }

        public static void CancelBooking(int id)
        {
            var booking = bookings.FirstOrDefault(x => x.Id == id);

            if (booking != null)
            {
                booking.Status = "Скасовано";
            }
        }
    }
}