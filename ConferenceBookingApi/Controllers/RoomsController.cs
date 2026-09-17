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
}