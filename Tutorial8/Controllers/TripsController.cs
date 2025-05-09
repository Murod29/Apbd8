using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripsService _tripsService;

        public TripsController(ITripsService tripsService)
        {
            _tripsService = tripsService;
        }

        /// <summary>
        /// Retrieves all trips from the database.
        /// </summary>
        /// <returns>List of trips with basic details</returns>
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripsService.GetTrips();
            return Ok(trips);
        }

        /// <summary>
        /// Retrieves a specific trip by ID.
        /// </summary>
        /// <param name="id">The ID of the trip to retrieve</param>
        /// <returns>The requested trip or 404 if not found</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrip(int id)
        {
            var trip = await _tripsService.GetTripById(id);
            if (trip == null)
                return NotFound($"Trip with id {id} not found");

            return Ok(trip);
        }
    }
}
