# ShopSolution

ShopSolution is a RESTful e-commerce API built with .NET 9. It provides endpoints for managing products, supports partial updates, implements API versioning, and demonstrates Clean Architecture principles.

---
## Project Goal

- Provide a REST API service listing all available products and allowing partial updates (stock changes) for individual products.
- Deliver complete Swagger documentation for all endpoints.
- Include unit tests covering endpoint functionality.
- Follow production-quality standards and best practices for REST API and SOLID application design.

---
## Architectural Decisions

- Flexible Product Creation: To prevent products from being permanently stuck at $0 (since no "Update Price" endpoint was requested), the Creation endpoint accepts Price and Description as optional fields. This strictly satisfies the requirement to allow creation with only Name and Image, while ensuring the system remains usable.
- Async Queue Strategy (V2): Used .NET Channels for background processing instead of external brokers like RabbitMQ. This demonstrates thread-safe, high-performance Producer/Consumer patterns without requiring the reviewer to set up external dependencies like Docker.
- Versioning: Implemented using Asp.Versioning.Mvc to support parallel execution of V1 (Synchronous) and V2 (Asynchronous/Paged) endpoints.
---
## Technical Specification

- Framework: .NET 9 and C# 13 (ASP.NET Core).
- Architecture: Layered Clean Architecture (Domain, Application, Infrastructure, API).
- Database: SQLite (Local file) with Entity Framework Core.
- Validation: FluentValidation for strict input control.
- Testing: xUnit, Moq, and AutoFixture.

---

## Data Specification

The `Product` entity includes:
- **Name**: Name of the product
- **ImgUrl**: URL to the main product image
- **Price**: Product price
- **Description**: Description of the product
- **StockQuantity**: Quantity of product stocked

Product creation requires only `name` and `imgUrl`.

---

## Data Layer

- Uses SQLite as the database (local file).
- Entity Framework Core as ORM.
- Initial database seeded with sample products.
- Mock data and in-memory database support for unit tests.

---

## API Specification

### Endpoints

- **List all products**  
  `GET /api/v1/products`  
  Returns all available products.

- **Create new product**  
  `POST /api/v1/products`  
  Requires `name` and `imgUrl`.

- **Get product by ID**  
  `GET /api/v1/products/{id}`  
  Returns details for a single product.

- **Update product stock**  
  `PATCH /api/v1/products/{id}/stock`  
  Updates only the stock quantity.

### Version 2 Features

- **Pagination**  
  `GET /api/v2/products?page=1&pageSize=10`  
  Returns paged results with `Items`, `TotalCount`, `Page`, and `PageSize`.

- **Async Stock Update**  
  `PATCH /api/v2/products/{id}/stock`  
  Uses an in-memory queue for asynchronous stock updates. Returns HTTP 202 Accepted.

---

## API Documentation

- Swagger UI is available at `https://localhost:5034/swagger` after running the API.
- Both v1 and v2 endpoints are documented and accessible.

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022
- Git

---

## Setup & Installation

1. **Clone the repository:**
`git clone https://github.com/00DRUG/ShopSolution.git cd ShopSolution`

2. **Restore dependencies:**
`dotnet restore`

3. **Build the solution:**
`dotnet build`

---

## Running the Application

1. **Start the API:**
`dotnet run --project Shop.Api`
The API will be available at `https://localhost:5034`.

2. **Access Swagger UI:**
   Open your browser and navigate to `https://localhost:5034/swagger`.

---

## API Usage Examples

See `Shop.Api/Shop.Api.http` for ready-to-use HTTP requests.

- **Get all products (v1):**
  `GET /api/v1/products`

- **Create a new product:**
`POST /api/v1/products { "name": "Super Gaming Laptop", "imgUrl": "https://example.com/laptop.jpg" }`

- **Update stock (v2, async):**
`PATCH /api/v2/products/1/stock { "newQuantity": 99 }`

---

## Running Unit Tests

1. **Run all tests:**
   `dotnet test`

2. **Run tests for a specific project:**
`dotnet test Shop.Tests`

Test results are shown in the terminal and in Visual Studio’s Test Explorer.

---

## Project Structure
The solution follows **Clean Architecture** principles to ensure separation of concerns and testability.
```text
ShopSolution/
├── Shop.Api/                     # Presentation Layer (Web API)
│   ├── Controllers/              # V1 and V2 Endpoints
│   ├── Program.cs                # DI Configuration & Middleware
│   └── Data/                     # SQLite database location
│
├── Shop.Application/             # Application Layer (Business Logic)
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Services/                 # Orchestration logic
│   └── Validators/               # FluentValidation rules
│
├── Shop.Domain/                  # Domain Layer (Core Business Rules)
│   ├── Product.cs                # Main Entity
│   └── Interfaces/               # Repository Contracts
│
├── Shop.Infrastructure/          # Infrastructure Layer (External concerns)
│   ├── Repositories/             # EF Core Implementations
│   └── BackgroundJobs/           # Async Queue (Channels)
│
└── Shop.Tests/                   # Unit Tests (xUnit, Moq, AutoFixture)

## Technologies Used

- .NET 9, C# 13
- ASP.NET Core
- Entity Framework Core (SQLite)
- FluentValidation
- Asp.Versioning
- Swagger (Swashbuckle)
- xUnit (for testing)
- In-memory queue for async processing
- AutoFixture

---

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

---
