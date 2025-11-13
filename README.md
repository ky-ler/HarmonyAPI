# HarmonyAPI

HarmonyAPI serves as the backend for a chat application, similar to Discord or Slack.

## Technology Stack

  * **.NET 8**
  * **ASP.NET Core Web API**
  * **Entity Framework Core**
  * **PostgreSQL**
  * **Auth0**
  * **Swagger / OpenAPI**

## Authentication & Authorization

Authentication is handled with Auth0. Endpoints are secured and may require specific permissions (scopes) in the access token. This is managed by the `HasScopeHandler`.

## Getting Started

### Prerequisites

  * .NET 8 SDK
  * A running PostgreSQL database
  * An Auth0 Application configured for API authorization

### Configuration

1.  Clone the repository.
2.  The project requires configuration for the database and Auth0.
3.  Add your PostgreSQL connection string:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=harmony;Username=postgres;Password=your_password"
      }
    }
    ```
4.  Add your Auth0 domain and audience:
    ```json
    {
      "Auth0": {
        "Domain": "your-auth0-domain.us.auth0.com",
        "Audience": "https://api.harmony.com"
      }
    }
    ```

### Running the Application

1.  Open a terminal in the `Api` directory or open the `Api.sln` solution file in your IDE.
2.  Run `dotnet restore` to install dependencies.
3.  Run `dotnet ef database update` to apply the Entity Framework migrations to your database.
4.  Run `dotnet run` to start the application.
5.  The API will be available at `http://localhost:5000` (or as specified in `launchSettings.json`)
6.  You can access the Swagger UI at `http://localhost:5000/swagger` to view and test the endpoints.
