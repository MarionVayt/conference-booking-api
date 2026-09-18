using System.Data;

namespace ConferenceBookingApi.Models;

public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public int DurationInHours  { get; set; }
    public decimal TotalPrice { get; set; }
}