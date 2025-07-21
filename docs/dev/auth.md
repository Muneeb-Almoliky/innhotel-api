# Authentication Documentation

## Overview

The InnHotel API uses JWT (JSON Web Token) based authentication with a refresh token mechanism. The system implements authentication flow with:
- Access tokens for API authorization
- Secure HTTP-only cookies for refresh tokens
- Role-based authorization
- Standardized error handling

## Authentication Flow

### 1. Login Flow (`POST /api/auth/login`)

```
Client                              API                               Database
  │                                  │                                   │
  │ POST /api/auth/login             │                                   │
  │ {email, password}                │                                   │
  │─────────────────────────────────>│                                   │
  │                                  │                                   │
  │                                  │ Check credentials                 │
  │                                  │──────────────────────────────────>│
  │                                  │                                   │
  │                                  │ User + Roles                      │
  │                                  │<──────────────────────────────────│
  │                                  │                                   │
  │                                  │ Generate tokens                   │
  │                                  │ Store refresh token               │
  │                                  │──────────────────────────────────>│
  │                                  │                                   │
  │ 200 OK                           │                                   │
  │ + Access Token (response body)   │                                   │
  │ + Refresh Token (httpOnly cookie)│                                   │
  │<─────────────────────────────────│                                   │
```

### 2. Token Refresh Flow (`POST /api/auth/refresh`)

```
Client                              API                               Database
  │                                  │                                   │
  │ POST /api/auth/refresh           │                                   │
  │ + Refresh Token (cookie)         │                                   │
  │─────────────────────────────────>│                                   │
  │                                  │ Validate refresh token            │
  │                                  │──────────────────────────────────>│
  │                                  │                                   │
  │                                  │ Token status                      │
  │                                  │<──────────────────────────────────│
  │                                  │                                   │
  │                                  │ Generate new tokens               │
  │                                  │ Update refresh token              │
  │                                  │──────────────────────────────────>│
  │                                  │                                   │
  │ 200 OK                           │                                   │
  │ + New Access Token               │                                   │
  │ + New Refresh Token (cookie)     │                                   │
  │<─────────────────────────────────│                                   │
```

## Security Features

1. **Token Security**
   - Access tokens are short-lived (15 minutes)
   - Refresh tokens use secure HTTP-only cookies
   - CSRF protection via cookie attributes
   - Token rotation on refresh

2. **Password Security**
   - Uses ASP.NET Identity for password hashing
   - Secure credential validation
   - Rate limiting on auth endpoints

3. **Authorization**
   - Role-based access control
   - Policy-based authorization
   - Custom policies (e.g., "AdminsOnly")

## Key Components

### 1. Web Layer (`InnHotel.Web.Auth`)
- `Login.cs`: Handles user authentication
- `RefreshToken.cs`: Manages token refresh
- Global auth error handling in `Program.cs`

### 2. Use Cases Layer (`InnHotel.UseCases.Auth`)
- `LoginHandler.cs`: Business logic for authentication
- `AuthResult.cs`: Internal auth result model

### 3. Core Layer
- `TokenService.cs`: JWT token generation and validation
- `AuthResponse.cs`: Public auth response model

## Error Handling

The API provides consistent error responses for authentication failures:

1. **Invalid Credentials**
   - Status: 401
   - Message: "Invalid Credentials"

2. **Invalid/Expired Token**
   - Status: 401
   - Message: "Unauthorized: authentication is required."

3. **Missing Refresh Token**
   - Status: 401
   - Message: "Refresh token is required."