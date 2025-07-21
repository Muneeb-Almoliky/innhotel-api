# API Layers

## Layer Interactions Flow

To understand how layers communicate, let's follow a request through the system using a room reservation example:

### Request Flow (Inbound)

```
Client (HTTP POST /api/reservations)
    │
    ▼
[1] Web Layer (API Endpoint)
    │  • Validates request format
    │  • Maps request to command/query
    │  • Handles authentication/authorization
    │
    ▼
[2] Use Cases Layer (Command/Query Handler)
    │  • Orchestrates the operation
    │  • Applies business logic
    │  • Coordinates with services
    │
    ▼
[3] Core Layer (Domain Logic)
    │  • Enforces business rules
    │  • Validates domain constraints
    │  • Maintains data integrity
    │
    ▼
[4] Infrastructure Layer
    │  • Executes database operations
    │  • Handles external services
    └──• Manages persistence
```

### Response Flow (Outbound)

```
Infrastructure Layer
    │  • Returns query results
    │  • Provides operation status
    │
    ▼
Core Layer
    │  • Validates domain state
    │  • Ensures business rules
    │
    ▼
Use Cases Layer
    │  • Transforms domain results
    │  • Handles errors/exceptions
    │  • Maps to response DTOs
    │
    ▼
Web Layer
    │  • Formats HTTP response
    │  • Sets status codes
    │  • Handles response headers
    │
    ▼
Client Response
```

### Real-World Example: Creating a Room Reservation

1. **Incoming Request**
   ```
   POST /api/reservations
   {
     "guestId": 123,
     "roomId": 456,
     "checkIn": "2024-04-01",
     "checkOut": "2024-04-05"
   }
   ```

2. **Layer-by-Layer Processing**

   a. **Web Layer** -> Request Entry
      - Receives HTTP POST request
      - Validates request format
      - Creates CreateReservationCommand
      - Passes to Use Cases layer

   b. **Use Cases Layer** -> Business Logic
      - Receives command
      - Checks if guest exists
      - Verifies room exists
      - Coordinates with services
      - Manages transaction

   c. **Core Layer** -> Domain Rules
      - Validates business rules:
        • Check-in before check-out
        • Valid reservation dates
        • Room availability
      - Maintains domain integrity

   d. **Infrastructure Layer** -> External Systems
      - Checks room availability in database
      - Persists reservation
      - Handles transaction completion

3. **Response Journey**

   a. **Infrastructure** <- Data Access
      - Returns operation result
      - Provides created reservation data

   b. **Core** <- Domain Validation
      - Ensures final state is valid
      - Confirms business rules

   c. **Use Cases** <- Response Preparation
      - Maps to ReservationResponse
      - Handles success/failure
      - Prepares response data

   d. **Web** <- HTTP Response
      - Sets appropriate status code (201 Created)
      - Formats JSON response
      - Adds response headers

4. **Final Response**
   ```
   201 Created
   {
     "status": 201,
     "message": "Reservation created successfully",
     "data": {
       "reservationId": 789,
       "guestId": 123,
       "roomId": 456,
       "checkIn": "2024-04-01",
       "checkOut": "2024-04-05"
     }
   }
   ```

### Key Communication Principles

1. **Dependency Direction**
   - Dependencies always point inward
   - Outer layers depend on inner layers
   - Core layer has no external dependencies

2. **Data Transformation**
   - Each layer transforms data for its needs
   - DTOs at boundaries between layers
   - Domain objects in Core layer

3. **Error Handling**
   - Each layer handles its specific errors
   - Errors propagate outward
   - Web layer translates to HTTP status codes

4. **Separation of Concerns**
   - Each layer has distinct responsibilities
   - No layer bypasses others
   - Clear boundaries maintained
