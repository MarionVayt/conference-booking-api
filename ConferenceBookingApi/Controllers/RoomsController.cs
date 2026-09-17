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
}