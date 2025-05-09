# Travel Agency REST API

This is a RESTful Web API for managing trips and clients in a travel agency system.

## 🔧 Technologies Used

- ASP.NET Core
- ADO.NET (SqlConnection / SqlCommand)
- SQL Server
- Rider or Visual Studio
- No Entity Framework

## 📂 Project Structure

- `Controllers/` – REST endpoints (e.g. TripsController)
- `Services/` – Business logic with raw SQL via ADO.NET
- `Models/DTOs/` – Data Transfer Objects
- `sql/script.sql` – Database schema and sample data

## 🚀 Endpoints

### GET /api/trips
Returns all trips with countries.

### GET /api/trips/{id}
Returns a specific trip.

### POST /api/trips
Creates a new trip.

## 🧪 How to Run

1. Create a SQL Server database named `APBD`
2. Run `sql/script.sql` in SSMS or Azure Data Studio
3. Update connection string in `appsettings.json`
4. Run project in Rider
5. Use Postman to test endpoints

---

📁 This project meets all requirements of Tutorial 8.
