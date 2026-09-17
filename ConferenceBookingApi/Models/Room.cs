namespace ConferenceBookingApi.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BasePricePerHour { get; set; }
    
    public List<Service> Services { get; set; } = new();
}
