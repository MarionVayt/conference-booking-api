namespace ConferenceBookingApi.Models;

public class BookingRequest
{
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public int DurationInHours { get; set; }
    
    public List<int> ServiceIds { get; set; } = new();
}