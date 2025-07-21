# API Operations Implementation Guide

This guide explains how to implement CRUD operations in the InnHotel API following our established patterns and conventions.

## File Structure

For each operation (e.g., for a `Branch` entity), create the following files:

### Web Layer (`src/InnHotel.Web/{Entity}/`)
The Web layer is the entry point for all HTTP requests and handles API endpoints, request/response models, and input validation. This layer is responsible for:
- Converting HTTP requests into commands/queries
- Input validation and model binding
- HTTP response formatting
- API documentation (Swagger/OpenAPI)
- Cross-cutting concerns like authentication and request logging

Files to create:
- `{Operation}.cs` - The endpoint implementation
- `{Operation}.{Operation}{Entity}Request.cs` - The request model
- `{Operation}.{Operation}{Entity}Validator.cs` - Request validation (if needed)

### Use Cases Layer (`src/InnHotel.UseCases/{Entity}/{Operation}/`)
The Use Cases layer contains the application's business logic and orchestrates the flow of data between the Web layer and the Core layer. This layer:
- Implements the application's use cases and business rules
- Orchestrates domain objects to accomplish specific tasks
- Handles the application's business logic flow
- Manages transactions and unit of work
- Maps between DTOs and domain entities

Files to create:
- `{Operation}{Entity}Command.cs` or `{Operation}{Entity}Query.cs` - The command/query
- `{Operation}{Entity}Handler.cs` - The handler **implementation**

### Core Layer
The Core layer contains the business domain model and business rules. This is the heart of the application that:
- Defines the domain entities, value objects, and aggregates
- Contains domain-specific business rules and logic
- Defines interfaces that are implemented by outer layers
- Houses specifications for complex queries
- Remains independent of external concerns

Files to create:
- For specifications: `src/InnHotel.Core/{Entity}Aggregate/Specifications/{Entity}By*Spec.cs`
- For services: `src/InnHotel.Core/Interfaces/I{Operation}{Entity}Service.cs`

### Infrastructure Layer
The Infrastructure layer provides implementations for interfaces defined in the Core layer and handles external concerns. This layer:
- Implements data persistence (repositories)
- Handles external service integrations
- Manages file system operations
- Implements core interfaces
- Provides technical services (email, logging, etc.)

Files to create:
- Service implementations: `src/InnHotel.Infrastructure/Services/{Operation}{Entity}Service.cs`

## Operation Examples

### List Operation
```
src/InnHotel.Web/{Entity}/
├── List.cs                           # Endpoint using PaginationRequest
└── List.List{Entity}Request.cs       # Route definition

src/InnHotel.UseCases/{Entity}/List/
├── List{Entity}Query.cs              # Query with pagination params
└── List{Entity}Handler.cs            # Handler using IReadRepository
```

### Create Operation
```
src/InnHotel.Web/{Entity}/
├── Create.cs                         # Endpoint implementation
└── Create.Create{Entity}Request.cs   # Request model with properties

src/InnHotel.UseCases/{Entity}/Create/
├── Create{Entity}Command.cs          # Command with properties
└── Create{Entity}Handler.cs          # Handler using IRepository
```

### GetById Operation
```
src/InnHotel.Web/{Entity}/
├── GetById.cs                        # Endpoint implementation
└── GetById.Get{Entity}ByIdRequest.cs # Request with ID property

src/InnHotel.UseCases/{Entity}/Get/
├── Get{Entity}Query.cs               # Query with ID
└── Get{Entity}Handler.cs             # Handler using IReadRepository

src/InnHotel.Core/{Entity}Aggregate/Specifications/
└── {Entity}ByIdSpec.cs              # Specification for finding by ID
```

### Update Operation
```
src/InnHotel.Web/{Entity}/
├── Update.cs                         # Endpoint implementation
└── Update.Update{Entity}Request.cs   # Request with ID and properties

src/InnHotel.UseCases/{Entity}/Update/
├── Update{Entity}Command.cs          # Command with properties
└── Update{Entity}Handler.cs          # Handler using IRepository
```

### Delete Operation
```
src/InnHotel.Web/{Entity}/
├── Delete.cs                         # Endpoint implementation
└── Delete.Delete{Entity}Request.cs   # Request with ID property

src/InnHotel.UseCases/{Entity}/Delete/
├── Delete{Entity}Command.cs          # Command with ID
└── Delete{Entity}Handler.cs          # Handler using service

src/InnHotel.Core/Interfaces/
└── IDelete{Entity}Service.cs         # Service interface

src/InnHotel.Infrastructure/Services/
└── Delete{Entity}Service.cs          # Service implementation
```

## Response Format Standards

### Success Responses

All success responses should follow this format:
```json
{
    "status": 200,  // HTTP status code
    "message": "Operation completed successfully",
    "data": {       // The actual response data
      // ... entity-specific properties
    }
}
```

For paginated responses:
```json
{
    "status": 200,
    "message": "Items retrieved successfully",
    "data": {
        "items": [...],
        "totalCount": 100,
        "pageNumber": 1,
        "pageSize": 10
    }
}
```

### Error Responses

All error responses should use the `FailureResponse` model:
```json
{
    "status": 400,    // HTTP status code
    "message": "Error message describing what went wrong",
    "details": [      // Optional array of detailed error messages
        "Validation error 1",
        "Validation error 2"
    ]
}
```

Common status codes:
- 200: Success
- 201: Created
- 400: Bad Request (validation errors)
- 404: Not Found
- 500: Server Error

## Tips

1. **Route Naming**:
   - Use consistent route patterns: `api/{Entity}s`
   - For ID-based operations: `api/{Entity}s/{id:int}`

2. **Validation**:
   - Add validators for create/update operations
   - Use data annotations for basic validation
   - Use FluentValidation for complex rules

3. **Repository Usage**:
   - Use `IReadRepository<T>` for read operations
   - Use `IRepository<T>` for write operations
   - Use specifications for complex queries

4. **Response Handling**:
   - Always use the standard response format
   - Use `FailureResponse` for all errors
   - Include meaningful messages
   - Add details array for validation errors

5. **Domain Events**:
   - Use services (like `IDelete{Entity}Service`) when you need to handle domain events
   - Keep handlers focused on basic CRUD operations

6. **Testing**:
   - Add HTTP tests in `http/tests/{entity}.http`
   - Test all response scenarios (success, validation, not found, etc.)