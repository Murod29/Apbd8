using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";

    /// <summary>
    /// Executes SQL to return all trips and their names from the Trip table.
    /// </summary>
    /// <returns>List of TripDTO with Id and Name</returns>
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command = "SELECT IdTrip, Name FROM Trip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    trips.Add(new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(1),
                    });
                }
            }
        }

        return trips;
    }

    /// <summary>
    /// Executes SQL to return a specific trip by its ID.
    /// </summary>
    /// <param name="id">Trip ID</param>
    /// <returns>TripDTO or null if not found</returns>
    public async Task<TripDTO?> GetTripById(int id)
    {
        // Implementation...
        return null;
    }

    /// <summary>
    /// Executes an INSERT statement to add a new trip.
    /// </summary>
    /// <param name="trip">TripDTO to insert</param>
    /// <returns>True if insertion was successful, false otherwise</returns>
    public async Task<bool> CreateTrip(TripDTO trip)
    {
        // Implementation...
        return false;
    }
}
