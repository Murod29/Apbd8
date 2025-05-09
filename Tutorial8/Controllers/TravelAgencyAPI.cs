// TravelAgencyController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TravelAgencyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ClientsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("/api/trips")]
        public async Task<IActionResult> GetAllTrips()
        {
            var trips = new List<object>();
            using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            using var command = new SqlCommand(@"
                SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name AS Country
                FROM Trip t
                JOIN Country_Trip ct ON ct.IdTrip = t.IdTrip
                JOIN Country c ON ct.IdCountry = c.IdCountry", connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                trips.Add(new
                {
                    Id = reader["IdTrip"],
                    Name = reader["Name"],
                    Description = reader["Description"],
                    DateFrom = reader["DateFrom"],
                    DateTo = reader["DateTo"],
                    MaxPeople = reader["MaxPeople"],
                    Country = reader["Country"]
                });
            }
            return Ok(trips);
        }

        [HttpGet("/api/clients/{id}/trips")]
        public async Task<IActionResult> GetClientTrips(int id)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await connection.OpenAsync();

            var checkCmd = new SqlCommand("SELECT 1 FROM Client WHERE IdClient = @Id", connection);
            checkCmd.Parameters.AddWithValue("@Id", id);
            if (await checkCmd.ExecuteScalarAsync() == null)
                return NotFound("Client not found");

            var cmd = new SqlCommand(@"
                SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, ct.RegisteredAt, ct.PaymentDate
                FROM Client_Trip ct
                JOIN Trip t ON ct.IdTrip = t.IdTrip
                WHERE ct.IdClient = @Id", connection);
            cmd.Parameters.AddWithValue("@Id", id);

            var trips = new List<object>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                trips.Add(new
                {
                    Id = reader["IdTrip"],
                    Name = reader["Name"],
                    Description = reader["Description"],
                    DateFrom = reader["DateFrom"],
                    DateTo = reader["DateTo"],
                    MaxPeople = reader["MaxPeople"],
                    RegisteredAt = reader["RegisteredAt"],
                    PaymentDate = reader["PaymentDate"]
                });
            }
            return Ok(trips);
        }

        [HttpPost("/api/clients")]
        public async Task<IActionResult> CreateClient([FromBody] ClientRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Telephone) ||
                string.IsNullOrWhiteSpace(request.Pesel))
                return BadRequest("Missing required fields");

            using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            using var command = new SqlCommand(@"
                INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
                OUTPUT INSERTED.IdClient
                VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)", connection);

            command.Parameters.AddWithValue("@FirstName", request.FirstName);
            command.Parameters.AddWithValue("@LastName", request.LastName);
            command.Parameters.AddWithValue("@Email", request.Email);
            command.Parameters.AddWithValue("@Telephone", request.Telephone);
            command.Parameters.AddWithValue("@Pesel", request.Pesel);

            await connection.OpenAsync();
            var newId = (int)await command.ExecuteScalarAsync();
            return Created($"/api/clients/{newId}", new { Id = newId });
        }

        [HttpPut("/api/clients/{id}/trips/{tripId}")]
        public async Task<IActionResult> RegisterClientForTrip(int id, int tripId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await connection.OpenAsync();

            var cmd = new SqlCommand(@"
                SELECT COUNT(*) FROM Trip t
                JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip
                WHERE t.IdTrip = @TripId", connection);
            cmd.Parameters.AddWithValue("@TripId", tripId);
            int count = (int)await cmd.ExecuteScalarAsync();

            cmd = new SqlCommand("SELECT MaxPeople FROM Trip WHERE IdTrip = @TripId", connection);
            cmd.Parameters.AddWithValue("@TripId", tripId);
            int max = (int)(await cmd.ExecuteScalarAsync() ?? 0);

            if (count >= max)
                return BadRequest("Trip is full");

            var checkClient = new SqlCommand("SELECT 1 FROM Client WHERE IdClient = @Id", connection);
            checkClient.Parameters.AddWithValue("@Id", id);
            if (await checkClient.ExecuteScalarAsync() == null)
                return NotFound("Client not found");

            var insertCmd = new SqlCommand(@"
                INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
                VALUES (@Id, @TripId, @Date)", connection);

            insertCmd.Parameters.AddWithValue("@Id", id);
            insertCmd.Parameters.AddWithValue("@TripId", tripId);
            insertCmd.Parameters.AddWithValue("@Date", DateTime.Now);

            await insertCmd.ExecuteNonQueryAsync();
            return Ok("Client registered for trip");
        }

        [HttpDelete("/api/clients/{id}/trips/{tripId}")]
        public async Task<IActionResult> UnregisterClientFromTrip(int id, int tripId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await connection.OpenAsync();

            var checkCmd = new SqlCommand("SELECT 1 FROM Client_Trip WHERE IdClient = @Id AND IdTrip = @TripId", connection);
            checkCmd.Parameters.AddWithValue("@Id", id);
            checkCmd.Parameters.AddWithValue("@TripId", tripId);

            if (await checkCmd.ExecuteScalarAsync() == null)
                return NotFound("Registration not found");

            var deleteCmd = new SqlCommand("DELETE FROM Client_Trip WHERE IdClient = @Id AND IdTrip = @TripId", connection);
            deleteCmd.Parameters.AddWithValue("@Id", id);
            deleteCmd.Parameters.AddWithValue("@TripId", tripId);

            await deleteCmd.ExecuteNonQueryAsync();
            return Ok("Client unregistered from trip");
        }
    }

    public class ClientRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Pesel { get; set; }
    }
}