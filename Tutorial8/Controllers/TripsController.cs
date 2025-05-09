/// <summary>
/// Returns all trips with their basic info and list of countries.
/// </summary>
/// <returns>List of TripDTO objects</returns>
[HttpGet]
public async Task<IActionResult> GetTrips()
{
    var trips = await _tripsService.GetTrips();
    return Ok(trips);
}

/// <summary>
/// Returns a single trip by ID.
/// </summary>
/// <param name="id">Trip ID</param>
/// <returns>TripDTO or 404 if not found</returns>
[HttpGet("{id}")]
public async Task<IActionResult> GetTrip(int id)
{
    var trip = await _tripsService.GetTripById(id);
    if (trip == null)
        return NotFound($"Trip with id {id} not found");

    return Ok(trip);
}

/// <summary>
/// Creates a new trip.
/// </summary>
/// <param name="trip">TripDTO with trip data</param>
/// <returns>200 OK or 400 BadRequest</returns>
[HttpPost]
public async Task<IActionResult> CreateTrip([FromBody] TripDTO trip)
{
    if (string.IsNullOrWhiteSpace(trip.Name) ||
        trip.DateFrom >= trip.DateTo ||
        trip.MaxPeople <= 0)
    {
        return BadRequest("Invalid trip input");
    }

    var success = await _tripsService.CreateTrip(trip);
    if (!success)
        return StatusCode(500, "Failed to insert trip");

    return Ok("Trip created successfully");
}
