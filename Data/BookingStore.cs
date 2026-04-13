using Luftreise_Command_project_.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Luftreise_Command_project_.Data
{
    public static class BookingStore
    {
        private static List<BookingTicket> tickets = new List<BookingTicket>
        {

        };
        public static void AddTicket(BookingTicket ticket)
        {
            tickets.Add(ticket);
        }
        public static List<BookingTicket> GetTicketsByEmail(string email)
        {
            return tickets.Where(t => t.UserEmail == email).ToList();
        }

        public static BookingTicket GetTicketById(int id)
        {
            return tickets.FirstOrDefault(t => t.Id == id);
        }


        public static List<BookingTicket> GetAllTickets()
        {
            return tickets;
        }

    }
}