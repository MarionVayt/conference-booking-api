using ConferenceBookingApi.Data;
using ConferenceBookingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBookingApi.Controllers;
[ApiController]
[Route("api/[controller]")]

public class RoomsController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoomsController(AppDbContext context)
    {
        _context = context;
    }
    private static List<Service> _services = new List<Service>
    {
        new Service { Id = 1, Name = "Проєктор", Price = 500 },
        new Service { Id = 2, Name = "Wi-Fi", Price = 300 },
        new Service { Id = 3, Name = "Звук", Price = 700 }
    };

    [HttpGet]
    public IActionResult GetAllRooms()
    {
        var rooms = _context.Rooms.ToList();
        return Ok(rooms);
    }
    
    [HttpPost]
    public IActionResult CreateRoom(Room room)
    {
        _context.Rooms.Add(room);
        _context.SaveChanges(); // Зберігаємо зміни у файл
        return CreatedAtAction(nameof(GetAllRooms), new { id = room.Id }, room);
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateRoom(int id, Room updatedRoom)
    {
        var existingRoom = _context.Rooms.FirstOrDefault(r => r.Id == id);
        if (existingRoom == null) return NotFound(new { Message = "Зал не знайдено" });
        
        existingRoom.Name = updatedRoom.Name;
        existingRoom.Capacity = updatedRoom.Capacity;
        existingRoom.BasePricePerHour = updatedRoom.BasePricePerHour;

        _context.SaveChanges();
        return Ok(existingRoom);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRoom(int id)
    {
        var existingRoom = _context.Rooms.FirstOrDefault(r => r.Id == id);
        if (existingRoom == null) return NotFound(new { Message = "Зал не знайдено" });

        _context.Rooms.Remove(existingRoom);
        _context.SaveChanges();
        return Ok(new { Message = "Зал успішно видалено" });
    }
    
    [HttpGet("bookings")]
    public IActionResult GetAllBookings()
    {
        var bookings = _context.Bookings.ToList();
        return Ok(bookings);
    }
    
    [HttpGet("available")]
    public IActionResult GetAvailableRooms(DateTime startDate, int durationInHours, int expectedCapacity)
    {
        var endDate = startDate.AddHours(durationInHours);

        var availableRooms = _context.Rooms
            .Where(r => r.Capacity >= expectedCapacity)
            .Where(r => !_context.Bookings.Any(b => 
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
        var room = _context.Rooms.FirstOrDefault(r => r.Id == request.RoomId);
        if (room == null) return NotFound(new { Message = "Зал не знайдено" });

        var requestEndDate = request.StartDate.AddHours(request.DurationInHours);

        bool isOccupied = _context.Bookings.Any(b => 
            b.RoomId == request.RoomId &&
            b.StartDate < requestEndDate &&
            b.StartDate.AddHours(b.DurationInHours) > request.StartDate
        );

        if (isOccupied)
        {
            return Conflict(new { Message = "Цей зал вже заброньовано на обраний час" });
        }

        decimal totalPrice = 0;
        DateTime currentHour = request.StartDate;

        for (int i = 0; i < request.DurationInHours; i++)
        {
            int hour = currentHour.Hour;
            decimal priceForThisHour = room.BasePricePerHour;

            if (hour >= 6 && hour < 9) { priceForThisHour = room.BasePricePerHour * 0.9m; }
            else if (hour >= 12 && hour < 14) { priceForThisHour = room.BasePricePerHour * 1.15m; }
            else if (hour >= 18 && hour < 23) { priceForThisHour = room.BasePricePerHour * 0.8m; }
        
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
            // Зверни увагу: Id тут більше немає, EF Core згенерує його сам!
            RoomId = room.Id,
            StartDate = request.StartDate,
            DurationInHours = request.DurationInHours,
            TotalPrice = totalPrice
        };

        _context.Bookings.Add(newBooking);
        _context.SaveChanges(); // Обов'язково зберігаємо в базу!

        return Ok(new { Message = "Бронювання успішне", TotalPrice = totalPrice, BookingId = newBooking.Id });
    }
}