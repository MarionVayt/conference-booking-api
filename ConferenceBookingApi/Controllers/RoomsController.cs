using ConferenceBookingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBookingApi.Controllers;
[ApiController]
[Route("api/[controller]")]

public class RoomsController : ControllerBase
{
    private static List<Room> _rooms = new List<Room>
    {
        new Room { Id = 1, Name = "Зал А", Capacity = 50, BasePricePerHour = 2000 },
        new Room { Id = 2, Name = "Зал В", Capacity = 100, BasePricePerHour = 3500 },
        new Room { Id = 3, Name = "Зал С", Capacity = 30, BasePricePerHour = 1500 }
    };
    
    private static List<Service> _services = new List<Service>
    {
        new Service { Id = 1, Name = "Проєктор", Price = 500 },
        new Service { Id = 2, Name = "Wi-Fi", Price = 300 },
        new Service { Id = 3, Name = "Звук", Price = 700 }
    };

    private static List<Booking> _bookings = new List<Booking>();

    [HttpGet]
    public IActionResult GetAllRooms()
    {
        return Ok(_rooms);
    }
    
    [HttpPost]
    public IActionResult CreateRoom(Room newRoom)
    {
        int newId = _rooms.Any() ? _rooms.Max(r => r.Id) + 1 : 1;
        newRoom.Id = newId;
        
        _rooms.Add(newRoom);
        
        return Ok(new { Message = "Зал успішно створено", RoomId = newId });
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateRoom(int id, Room updatedRoom)
    {

        var existingRoom = _rooms.FirstOrDefault(r => r.Id == id);
        
        if (existingRoom == null)
        {
            return NotFound(new { Message = "Зал з таким ID не знайдено" });
        }
        
        existingRoom.Name = updatedRoom.Name;
        existingRoom.Capacity = updatedRoom.Capacity;
        existingRoom.BasePricePerHour = updatedRoom.BasePricePerHour;

        return Ok(new { Message = "Інформацію про зал успішно оновлено" });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRoom(int id)
    {
        var existingRoom = _rooms.FirstOrDefault(r => r.Id == id);
        if (existingRoom == null)
        {
            return NotFound(new { Message = "Зал з таким ID не знайдено" });
        }
        
        _rooms.Remove(existingRoom);
        return Ok(new {Message = "Зал успішно видалено"});
    }
    
    [HttpGet("available")]
    public IActionResult GetAvailableRooms(DateTime  startDate, int durationInHours, int expectedCapacity)
    {
        var endDate = startDate.AddHours(durationInHours);

        var availableRooms = _rooms
            .Where(r => r.Capacity >= expectedCapacity)
            .Where(r => !_bookings.Any(b => 
                    b.RoomId == r.Id &&
                    b.StartDate < endDate &&
                    b.StartDate.AddHours(b.DurationInHours) > startDate
            ))
            .ToList();
        return Ok(availableRooms);
    }
    
    [HttpPost("book")]
    public IActionResult BookRoom(BookingRequest request)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == request.RoomId);
        if (room == null) return NotFound(new { Message = "Зал не знайдено" });

        decimal totalPrice = 0;
        DateTime currentHour = request.StartDate;

        for (int i = 0; i < request.DurationInHours; i++)
        {
            int hour = currentHour.Hour;
            decimal priceForThisHour = room.BasePricePerHour;

            if (hour >= 6 && hour < 9)
            {
                priceForThisHour = room.BasePricePerHour * 0.9m;
            }
            else if (hour >= 12 && hour < 14)
            {
                priceForThisHour = room.BasePricePerHour * 1.15m;
            }
            else if (hour >= 18 && hour < 23)
            {
                priceForThisHour = room.BasePricePerHour * 0.8m;
            }
            else
            {
                priceForThisHour = room.BasePricePerHour;
            }

            totalPrice += priceForThisHour;
            currentHour = currentHour.AddHours(1);
        }
        
        foreach (var serviceId in request.ServiceIds)
        {
            var service = _services.FirstOrDefault(s => s.Id == serviceId);
            if (service != null)
            {
                totalPrice += service.Price;
            }
        }
        
        var newBooking = new Booking
        {
            Id = _bookings.Any() ? _bookings.Max(b => b.Id) + 1 : 1,
            RoomId = room.Id,
            StartDate = request.StartDate,
            DurationInHours = request.DurationInHours,
            TotalPrice = totalPrice
        };
        _bookings.Add(newBooking);

        return Ok(new { Message = "Бронювання успішне", TotalPrice = totalPrice, BookingId = newBooking.Id });
    }

    [HttpGet("bookings")]
    public IActionResult GetAllBookings()
    {
        return Ok(_bookings);
    }
}