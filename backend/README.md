# eShop Backend

This is the backend of the eShop project, a modular e-commerce system built with .NET 9. It follows Clean Architecture, Domain-Driven Design (DDD), and CQRS principles to ensure maintainability and scalability.

## Features
- RESTful API for product management (CRUD, filtering, sorting).
- Caching with Redis.
- Photo storage with Azure Blob Storage.
- Automatic MSSQL database migrations.
- Middleware for error handling, CSRF protection, CORS, and rate limiting.
- Logging with Serilog.
- API documentation with Swagger.

## Technologies
- **Languages**: C#
- **Framework**: .NET 9
- **Database**: MSSQL
- **Caching**: Redis
- **Storage**: Azure Blob Storage
- **Libraries**: MediaTR, AutoMapper, FluentValidation, Serilog, Swagger
- **Tools**: Docker, Git, GitHub Actions

## Project Structure
```
backend/
├── eShop.Host/              # Entry point and API configuration
├── eShop.Infrastructure/    # Infrastructure layer (DB, caching, storage)
├── eShop.Modules.Catalog/   # Catalog module (business logic)
├── eShop.Shared/            # Shared utilities and abstractions
├── .dockerignore            # Docker ignore file
├── .gitignore              # Git ignore file
└── eShop.sln                # Solution file
```

## Prerequisites
- Docker and Docker Compose
- .NET 9 SDK (for building locally, if needed)
- MSSQL Server or Dockerized MSSQL instance
- Redis (local or Dockerized)
- Azure Blob Storage account (or local emulator like Azurite)

## Setup and Installation
1. **Clone the repository**:
   ```bash
   git clone https://github.com/kacpersmaga/eShop.git
   cd eShop/backend
   ```
2. **Restore dependencies** (if building locally):
   ```bash
   dotnet restore eShop.sln
   ```
3. **Run with Docker**:
   ```bash
   cd ..  # Return to root directory
   docker-compose up --build
   ```
   - The API will be available at `http://localhost:8080` (or proxied via Caddy in production at `https://kacpersmaga.pl/api`).

## API Documentation
- Swagger UI is available at `http://localhost:8080/swagger` (or `https://kacpersmaga.pl/swagger` in production) when the application is running via Docker.

## Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Set to `Development` or `Production`.
- `ConnectionStrings__DefaultConnection`: MSSQL connection string (e.g., `Server=db;Database=eshop;User Id=sa;Password=Password123!;TrustServerCertificate=True`).
- `ConnectionStrings__Redis`: Redis connection string (e.g., `redis:6379,password=SuperSilneHaslo123!`).
- `AzureBlobStorage__ConnectionString`: Azure Blob Storage connection string.
- `PUBLIC_BLOB_HOST`: Public URL for blob storage (e.g., `https://kacpersmaga.pl/storage`).
- `FrontendOrigin`: CORS-allowed frontend origin (e.g., `https://kacpersmaga.pl`).
- `CADDY_PROXY_IP`: IP address for forwarded headers configuration (e.g., your VPS public IP).

## Author
Kacper Smaga  
- Email: kacper.smaga@onet.pl  
- GitHub: [kacpersmaga](https://github.com/kacpersmaga)