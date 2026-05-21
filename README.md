# Setup Database

## 1. Configure Connection String

Open `appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
}
2. Update Database using EF Core

Open Terminal in the project folder and run:

dotnet ef database update
