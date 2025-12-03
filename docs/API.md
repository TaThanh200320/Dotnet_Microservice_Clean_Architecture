# 📚 API Documentation

Tài liệu API cho các endpoints có sẵn trong hệ thống.

## Base URLs

- **Development**: `http://localhost:5001`
- **Production**: `https://api.yourdomain.com`
- **API Gateway**: `http://localhost` (via Nginx)

## API Versioning

API hỗ trợ versioning thông qua URL:

```
/api/v1/users
/api/v2/users
```

## Authentication

Hầu hết các endpoints yêu cầu authentication bằng JWT Bearer token.

### Header Format

```http
Authorization: Bearer <access_token>
```

### Get Access Token

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "550e8400-e29b-41d4-a716-446655440000",
    "expiresIn": 3600,
    "tokenType": "Bearer"
  }
}
```

## Response Format

### Success Response

```json
{
  "success": true,
  "data": {
    // Response data here
  },
  "message": "Operation completed successfully",
  "timestamp": "2024-11-28T10:30:00Z"
}
```

### Error Response

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Validation failed",
    "details": {
      "email": ["Email is required"],
      "password": ["Password must be at least 8 characters"]
    }
  },
  "timestamp": "2024-11-28T10:30:00Z"
}
```

### Pagination Response

```json
{
  "success": true,
  "data": {
    "items": [],
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalCount": 100,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

## Authentication Endpoints

### POST /api/v1/auth/register

Đăng ký user mới.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+84901234567"
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "id": "01JDKQW8X9Y0Z1A2B3C4D5E6F7",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

### POST /api/v1/auth/login

Đăng nhập.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "550e8400-e29b-41d4-a716-446655440000",
    "expiresIn": 3600,
    "tokenType": "Bearer"
  }
}
```

### POST /api/v1/auth/refresh

Refresh access token.

**Request:**
```json
{
  "accessToken": "expired_token",
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "650e8400-e29b-41d4-a716-446655440000",
    "expiresIn": 3600,
    "tokenType": "Bearer"
  }
}
```

### POST /api/v1/auth/logout

Đăng xuất (invalidate refresh token).

**Headers:**
```http
Authorization: Bearer <access_token>
```

**Request:**
```json
{
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

### POST /api/v1/auth/forgot-password

Yêu cầu reset password.

**Request:**
```json
{
  "email": "user@example.com"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "Password reset email sent"
}
```

### POST /api/v1/auth/reset-password

Reset password với token.

**Request:**
```json
{
  "token": "reset-token-from-email",
  "newPassword": "NewSecurePassword123!"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "Password reset successfully"
}
```

## User Endpoints

### GET /api/v1/users/me

Lấy thông tin user hiện tại.

**Headers:**
```http
Authorization: Bearer <access_token>
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "01JDKQW8X9Y0Z1A2B3C4D5E6F7",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+84901234567",
    "roles": ["User"],
    "isActive": true,
    "createdAt": "2024-11-28T10:00:00Z"
  }
}
```

### PUT /api/v1/users/me

Cập nhật thông tin user hiện tại.

**Headers:**
```http
Authorization: Bearer <access_token>
```

**Request:**
```json
{
  "firstName": "John",
  "lastName": "Smith",
  "phoneNumber": "+84901234567"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "01JDKQW8X9Y0Z1A2B3C4D5E6F7",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Smith",
    "phoneNumber": "+84901234567"
  }
}
```

### POST /api/v1/users/me/change-password

Đổi password.

**Headers:**
```http
Authorization: Bearer <access_token>
```

**Request:**
```json
{
  "currentPassword": "SecurePassword123!",
  "newPassword": "NewSecurePassword456!"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "Password changed successfully"
}
```

### GET /api/v1/users

Lấy danh sách users (Admin only).

**Headers:**
```http
Authorization: Bearer <access_token>
```

**Query Parameters:**
- `pageNumber` (int, default: 1): Page number
- `pageSize` (int, default: 20): Items per page
- `search` (string): Search by email or name
- `sortBy` (string): Sort field (email, firstName, createdAt)
- `sortOrder` (string): Sort order (asc, desc)
- `filter` (string): Dynamic filter (e.g., `isActive==true`)

**Example:**
```http
GET /api/v1/users?pageNumber=1&pageSize=20&search=john&sortBy=createdAt&sortOrder=desc&filter=isActive==true
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "01JDKQW8X9Y0Z1A2B3C4D5E6F7",
        "email": "john@example.com",
        "firstName": "John",
        "lastName": "Doe",
        "isActive": true,
        "createdAt": "2024-11-28T10:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalCount": 100,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

### GET /api/v1/users/{id}

Lấy thông tin user theo ID (Admin only).

**Headers:**
```http
Authorization: Bearer <access_token>
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "01JDKQW8X9Y0Z1A2B3C4D5E6F7",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+84901234567",
    "roles": ["User"],
    "isActive": true,
    "createdAt": "2024-11-28T10:00:00Z"
  }
}
```

### PUT /api/v1/users/{id}

Cập nhật user (Admin only).

**Headers:**
```http
Authorization: Bearer <access_token>
```

**Request:**
```json
{
  "firstName": "John",
  "lastName": "Smith",
  "phoneNumber": "+84901234567",
  "isActive": true,
  "roles": ["User", "Manager"]
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "01JDKQW8X9Y0Z1A2B3C4D5E6F7",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Smith",
    "roles": ["User", "Manager"]
  }
}
```

### DELETE /api/v1/users/{id}

Xóa user (Admin only).

**Headers:**
```http
Authorization: Bearer <access_token>
```

**Response:** `204 No Content`

## Health Check Endpoints

### GET /health

Basic health check.

**Response:** `200 OK`
```json
{
  "status": "Healthy",
  "checks": {
    "self": "Healthy",
    "database": "Healthy",
    "redis": "Healthy"
  },
  "totalDuration": "00:00:00.1234567"
}
```

### GET /health/ready

Readiness check (để Kubernetes biết service ready).

**Response:** `200 OK`

### GET /health/live

Liveness check (để Kubernetes biết service còn sống).

**Response:** `200 OK`

## Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `VALIDATION_ERROR` | 400 | Request validation failed |
| `UNAUTHORIZED` | 401 | Authentication required |
| `FORBIDDEN` | 403 | Insufficient permissions |
| `NOT_FOUND` | 404 | Resource not found |
| `CONFLICT` | 409 | Resource already exists |
| `INTERNAL_ERROR` | 500 | Internal server error |
| `SERVICE_UNAVAILABLE` | 503 | Service temporarily unavailable |

## Rate Limiting

API endpoints có rate limiting:

- **Anonymous requests**: 100 requests/minute
- **Authenticated requests**: 1000 requests/minute
- **Admin requests**: Unlimited

**Response khi bị rate limited:**
```json
{
  "success": false,
  "error": {
    "code": "RATE_LIMIT_EXCEEDED",
    "message": "Too many requests. Please try again later."
  }
}
```

**Headers:**
```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1701388800
Retry-After: 60
```

## Dynamic Query

Hệ thống hỗ trợ dynamic query với các operators:

### Filter Operators

- `==` Equal
- `!=` Not equal
- `>` Greater than
- `>=` Greater than or equal
- `<` Less than
- `<=` Less than or equal
- `@=` Contains
- `_=` Starts with
- `!@=` Not contains
- `!_=` Not starts with

### Examples

```http
# Email contains "john"
GET /api/v1/users?filter=email@=john

# Active users created after 2024-01-01
GET /api/v1/users?filter=isActive==true,createdAt>2024-01-01

# Users with role Admin or Manager
GET /api/v1/users?filter=roles@=Admin|Manager
```

### Sorting

```http
# Sort by createdAt descending
GET /api/v1/users?sortBy=createdAt&sortOrder=desc

# Sort by multiple fields
GET /api/v1/users?sortBy=lastName,firstName&sortOrder=asc,asc
```

## Pagination

Tất cả list endpoints đều hỗ trợ pagination:

```http
GET /api/v1/users?pageNumber=2&pageSize=50
```

**Response:**
```json
{
  "items": [...],
  "pageNumber": 2,
  "pageSize": 50,
  "totalPages": 10,
  "totalCount": 500,
  "hasPreviousPage": true,
  "hasNextPage": true
}
```

## Multi-tenancy

API hỗ trợ multi-tenancy. Gửi tenant ID qua header:

```http
X-Tenant-Id: tenant-id-here
```

Hoặc tenant sẽ được extract từ JWT token claim.

## CORS

API hỗ trợ CORS với các origins được whitelist.

**Allowed Methods:**
- GET
- POST
- PUT
- DELETE
- PATCH
- OPTIONS

**Allowed Headers:**
- Content-Type
- Authorization
- X-Tenant-Id
- X-Requested-With

## Swagger/OpenAPI

Interactive API documentation có sẵn tại:

- **Development**: <http://localhost:5001/docs>
- **Production**: <https://api.yourdomain.com/docs>

OpenAPI spec (JSON): `/swagger/v1/swagger.json`

## Examples với cURL

### Register & Login

```bash
# Register
curl -X POST http://localhost:5001/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePassword123!",
    "firstName": "John",
    "lastName": "Doe"
  }'

# Login
TOKEN=$(curl -X POST http://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePassword123!"
  }' | jq -r '.data.accessToken')

echo $TOKEN
```

### Authenticated Requests

```bash
# Get current user
curl -X GET http://localhost:5001/api/v1/users/me \
  -H "Authorization: Bearer $TOKEN"

# Update profile
curl -X PUT http://localhost:5001/api/v1/users/me \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Jane",
    "lastName": "Smith"
  }'
```

### List Users (Admin)

```bash
# Get all users with pagination
curl -X GET "http://localhost:5001/api/v1/users?pageNumber=1&pageSize=20" \
  -H "Authorization: Bearer $ADMIN_TOKEN"

# Search users
curl -X GET "http://localhost:5001/api/v1/users?search=john&filter=isActive==true" \
  -H "Authorization: Bearer $ADMIN_TOKEN"
```

## Postman Collection

Import Postman collection từ: `/docs/postman/STC-Microservice.postman_collection.json`

**Environment Variables:**
- `base_url`: `http://localhost:5001`
- `access_token`: (auto-set after login)
- `refresh_token`: (auto-set after login)

## Client Libraries

### JavaScript/TypeScript

```typescript
import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5001',
  headers: {
    'Content-Type': 'application/json'
  }
});

// Add token to requests
api.interceptors.request.use(config => {
  const token = localStorage.getItem('access_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Login
const { data } = await api.post('/api/v1/auth/login', {
  email: 'user@example.com',
  password: 'SecurePassword123!'
});

localStorage.setItem('access_token', data.data.accessToken);

// Get current user
const user = await api.get('/api/v1/users/me');
```

### C# (.NET)

```csharp
using System.Net.Http.Json;

var client = new HttpClient
{
    BaseAddress = new Uri("http://localhost:5001")
};

// Login
var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
{
    Email = "user@example.com",
    Password = "SecurePassword123!"
});

var loginData = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<TokenResponse>>();
var token = loginData.Data.AccessToken;

// Add token to requests
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

// Get current user
var userResponse = await client.GetAsync("/api/v1/users/me");
var user = await userResponse.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
```
