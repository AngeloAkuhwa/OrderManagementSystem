# Order Management System API

A .NET 8 Web API project simulating an order management system. This application showcases clean architecture, CQRS pattern with MediatR, Entity Framework Core, Swagger documentation, and comprehensive testing using xUnit and Moq.

---

## Features Implemented

### Discounting System

- Applies dynamic discounts based on `CustomerSegment` and `OrderHistory`.
- Uses the **Strategy Pattern** with these strategies:

  - New Customer Discount
  - Loyal Customer Discount
  - VIP Customer Discount

- Endpoint: `POST /api/v1/discount/{orderId}/apply-discount`

### Order Status Tracking

- State transitions: `Created → Processing → Shipped → Delivered`
- Invalid transitions throw appropriate errors.
- Endpoint: `PATCH /api/v1/orders/{id}/update-status`

### Order Analytics

- Returns:

  - Average Order Value
  - Fulfillment Rate
  - Average Fulfillment Time
  - Status Breakdown
  - Monthly Order Trend
  - Order Value Histogram
  - Recent Orders

- Endpoint: `GET /api/v1/orders/analytics`

---

## Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- IDE like Visual Studio 2022 or Rider

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/order-management-system-api.git
cd order-management-system-api
```

### 2. Restore Packages

```bash
dotnet restore
```

### 3. Apply Migrations

```bash
cd OrderManagementSystem.Presentation
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run --project OrderManagementSystem.Presentation
```

Visit Swagger UI:

```
http://localhost:5055/swagger/index.html
```

### 5. Run Tests

```bash
dotnet test
```

---

## Architecture

- **Clean Architecture**: Layered separation:

  - `Domain`: Entities, enums, core rules
  - `Application`: Features, CQRS handlers, interfaces
  - `Infrastructure`: EF Core setup, repositories, seeders
  - `Presentation`: Web API layer and DI registration
  - `Tests`: Unit and integration tests

- **MediatR + CQRS**:

  - `Commands` for state changes
  - `Queries` for data retrieval

- **Database**:

  - EF Core In-Memory (for tests)
  - EF Core SQL Server (for production-ready runs)

- **Swagger (OpenAPI)**:

  - Auto-docs with XML comments

---

## Feature Development Workflow

Each feature is broken into:

- **DTOs**: Request/Response contracts
- **Command/Query Handlers**: With `IRequestHandler<TRequest, TResponse>`
- **Service Abstractions**: Interfaces for domain logic
- **Strategies**: Discount strategies per customer segment

Example:

```csharp
public class ApplyDiscountHandler : IRequestHandler<ApplyDiscountRequest, ApplyDiscountResponse>
```

---

## Testing

### Unit Tests

- Located under `OrderManagementSystem.Tests.UnitTests`
- Includes:

  - Discount logic tests
  - Order analytics logic tests
  - State transition validator tests

### Integration Tests

- Located under `OrderManagementSystem.Tests.IntegrationTests`
- Tests:

  - `/apply-discount` endpoint
  - `/analytics` endpoint

---

## Sample API Payloads

### POST /api/v1/discount/{orderId}/apply-discount

Response:

```json
{
	"originalAmount": 200.0,
	"discountedAmount": 180.0,
	"discount": 20.0
}
```

### PATCH /api/v1/orders/{id}/update-status

Request:

```json
{
	"newStatus": "Shipped"
}
```

### GET /api/v1/orders/analytics

Response:

```json
{
  "summary": {
    "totalOrders": 20,
    "averageOrderValue": 235.5,
    "fulfillmentRate": 80,
    "averageFulfillmentTimeInHours": 12.3
  },
  "statusBreakdown": [ ... ],
  "monthlyOrderCounts": [ ... ],
  "valueRangeHistogram": [ ... ],
  "recentOrders": [ ... ]
}
```

### Logging

- Integrated **Serilog** for robust logging.
- Logs are written to both the console and daily rolling files under `Logs/log.txt`.
- Useful for tracing request execution, debugging failures, and tracking analytics computation steps.

### Global Exception Handling

- Centralized error handling using custom middleware.
- Unhandled exceptions are logged and returned as consistent error responses.
- Ensures API clients receive meaningful error messages.

---

## Assumptions Made

- Loyalty is based on 5+ past orders
- VIP total spend threshold is \$5000
- Only defined transitions are allowed
- Tests run with seeded data (using `DbSeeder` and `TestSeeder`)

---

## Tools & Libraries

- .NET 8
- MediatR
- Entity Framework Core (SQLServer + InMemory)
- Swashbuckle (Swagger)
- xUnit
- Moq
- FluentAssertions

---

## Future Improvements

- Add authentication (JWT)
- Implement pagination in analytics
- Deploy to Azure / Docker

---

## Author

Angelo Akuhwa
API Feature Implementation Assessment, May 2025
