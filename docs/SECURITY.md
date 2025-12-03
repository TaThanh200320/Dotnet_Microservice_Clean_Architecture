# 🔐 Security & Authentication

## Tổng quan

Hệ thống sử dụng JWT (JSON Web Tokens) cho authentication và authorization với các tính năng:

- Access Token & Refresh Token
- Role-based Authorization
- Claims-based Authorization
- Token Rotation
- Secure Password Storage
- Multi-tenant Security

## Authentication Flow

### 1. User Login

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant AuthService
    participant Database
    
    Client->>API: POST /api/auth/login
    API->>AuthService: Validate Credentials
    AuthService->>Database: Get User
    Database-->>AuthService: User Data
    AuthService->>AuthService: Verify Password
    AuthService->>AuthService: Generate Access Token
    AuthService->>AuthService: Generate Refresh Token
    AuthService->>Database: Save Refresh Token
    AuthService-->>API: Tokens
    API-->>Client: 200 OK + Tokens
```

**Request:**
```json
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000",
  "expiresIn": 3600,
  "tokenType": "Bearer"
}
```

### 2. Access Protected Endpoint

```http
GET /api/users/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 3. Refresh Token

Khi Access Token hết hạn, sử dụng Refresh Token để lấy token mới:

```json
POST /api/auth/refresh
{
  "accessToken": "expired_token",
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "650e8400-e29b-41d4-a716-446655440000",
  "expiresIn": 3600,
  "tokenType": "Bearer"
}
```

## JWT Configuration

### Token Structure

Access Token chứa các claims:

```json
{
  "sub": "01JDKQW8X9Y0Z1A2B3C4D5E6F7",     // User ID (ULID)
  "email": "user@example.com",
  "name": "John Doe",
  "tenant": "tenant-id",                   // Multi-tenant support
  "roles": ["Admin", "User"],
  "permissions": ["users.read", "users.write"],
  "jti": "unique-token-id",                // JWT ID
  "iat": 1701388800,                       // Issued At
  "exp": 1701392400,                       // Expiration
  "iss": "YourIssuer",                     // Issuer
  "aud": "YourAudience"                    // Audience
}
```

### Configuration (appsettings.json)

```json
{
  "JwtSettings": {
    "SecretKey": "your-256-bit-secret-key-here-must-be-long-enough",
    "Issuer": "YourIssuer",
    "Audience": "YourAudience",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ValidateIssuerSigningKey": true,
    "ClockSkew": 5
  }
}
```

### Environment Variables (.env)

```bash
# JWT Configuration
JWT_SECRET_KEY=your-very-long-secret-key-minimum-256-bits
JWT_ISSUER=IdentityService
JWT_AUDIENCE=MicroserviceClients
JWT_EXPIRATION_MINUTES=60
JWT_REFRESH_EXPIRATION_DAYS=7
```

## Generating Secure Keys

### Generate JWT Secret Key

```bash
# Sử dụng script có sẵn
./scripts/generate-jwt-keys.sh

# Hoặc manual với OpenSSL
openssl rand -base64 64
```

### Generate RSA Keys (Optional - for RSA256)

```bash
# Private key
openssl genrsa -out private.key 4096

# Public key
openssl rsa -in private.key -pubout -out public.key
```

## Authorization

### Role-based Authorization

#### Trong Controller

```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("api/users/{id}")]
public async Task<IActionResult> DeleteUser(Ulid id)
{
    // Only Admin can access
}

[Authorize(Roles = "Admin,Manager")]
[HttpGet("api/reports")]
public async Task<IActionResult> GetReports()
{
    // Admin or Manager can access
}
```

#### Trong Handler (MediatR)

```csharp
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUser _currentUser;
    
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        if (!_currentUser.IsInRole("Admin"))
            return Result.Failure(Error.Forbidden());
            
        // Delete user logic
    }
}
```

### Claims-based Authorization

#### Định nghĩa Policy

```csharp
builder.Services.AddAuthorization(options =>
{
    // Permission-based
    options.AddPolicy("CanReadUsers", policy =>
        policy.RequireClaim("permission", "users.read"));
        
    options.AddPolicy("CanWriteUsers", policy =>
        policy.RequireClaim("permission", "users.write"));
        
    // Complex policies
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin")
              .RequireClaim("email_verified", "true"));
              
    // Custom requirement
    options.AddPolicy("TenantAdmin", policy =>
        policy.Requirements.Add(new TenantAdminRequirement()));
});
```

#### Sử dụng Policy

```csharp
[Authorize(Policy = "CanReadUsers")]
[HttpGet("api/users")]
public async Task<IActionResult> GetUsers()
{
    // Requires "users.read" permission
}

[Authorize(Policy = "CanWriteUsers")]
[HttpPost("api/users")]
public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
{
    // Requires "users.write" permission
}
```

### Custom Authorization Handler

```csharp
public class TenantAdminRequirement : IAuthorizationRequirement { }

public class TenantAdminHandler : AuthorizationHandler<TenantAdminRequirement>
{
    private readonly ITenantContext _tenantContext;
    
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantAdminRequirement requirement)
    {
        var tenantClaim = context.User.FindFirst("tenant")?.Value;
        
        if (context.User.IsInRole("Admin") && 
            tenantClaim == _tenantContext.TenantId)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
```

## Password Security

### Password Hashing

```csharp
using Microsoft.AspNetCore.Identity;

public class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    
    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }
    
    public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            user, 
            hashedPassword, 
            providedPassword);
            
        return result == PasswordVerificationResult.Success;
    }
}
```

### Password Requirements

```csharp
builder.Services.AddIdentityCore<User>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 4;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // User settings
    options.User.RequireUniqueEmail = true;
});
```

### Password Validation

```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit")
            .Matches(@"[\W]").WithMessage("Password must contain at least one special character");
    }
}
```

## Multi-tenant Security

### Tenant Isolation

```csharp
public class TenantMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Extract tenant from header
        var tenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        
        // Or from JWT claim
        if (string.IsNullOrEmpty(tenantId))
        {
            tenantId = context.User.FindFirst("tenant")?.Value;
        }
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            _tenantContext.SetTenant(tenantId);
        }
        
        await _next(context);
    }
}
```

### Query Filter

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Global query filter for tenant isolation
    modelBuilder.Entity<User>().HasQueryFilter(e => 
        !_tenantContext.HasTenant || e.TenantId == _tenantContext.TenantId);
        
    modelBuilder.Entity<Order>().HasQueryFilter(e => 
        !_tenantContext.HasTenant || e.TenantId == _tenantContext.TenantId);
}
```

### Tenant Validation

```csharp
public class TenantValidator : IAuthorizationRequirement
{
    public async Task ValidateAsync(AuthorizationHandlerContext context)
    {
        var tenantId = context.User.FindFirst("tenant")?.Value;
        
        if (string.IsNullOrEmpty(tenantId))
        {
            context.Fail();
            return;
        }
        
        // Validate tenant exists and is active
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
        if (tenant == null || !tenant.IsActive)
        {
            context.Fail();
            return;
        }
        
        context.Succeed(this);
    }
}
```

## API Security

### CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
                "https://yourdomain.com",
                "https://app.yourdomain.com"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
    
    // For development only
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Usage
app.UseCors(isDevelopment ? "AllowAll" : "AllowSpecificOrigins");
```

### Rate Limiting

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            });
    });
});

app.UseRateLimiter();
```

### Request Size Limits

```csharp
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
});
```

### Security Headers

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add(
        "Content-Security-Policy", 
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'");
    
    await next();
});
```

## HTTPS Configuration

### Development Certificate

```bash
# Generate development certificate
dotnet dev-certs https --trust
```

### Production Certificate

```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 443, listenOptions =>
    {
        listenOptions.UseHttps("/path/to/certificate.pfx", "password");
    });
});
```

### Enforce HTTPS

```csharp
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

app.UseHttpsRedirection();
app.UseHsts();
```

## Audit Logging

### Audit Entity

```csharp
public class AuditLog
{
    public Ulid Id { get; set; }
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public string Action { get; set; }
    public string EntityName { get; set; }
    public string EntityId { get; set; }
    public string Changes { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public string TenantId { get; set; }
}
```

### Audit Interceptor

```csharp
public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, ct);
        
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added || 
                entry.State == EntityState.Modified ||
                entry.State == EntityState.Deleted)
            {
                // Log audit entry
                var audit = new AuditLog
                {
                    UserId = _currentUser.Id,
                    UserEmail = _currentUser.Email,
                    Action = entry.State.ToString(),
                    EntityName = entry.Entity.GetType().Name,
                    EntityId = GetEntityId(entry.Entity),
                    Changes = JsonSerializer.Serialize(entry.CurrentValues.Properties),
                    IpAddress = _httpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = _httpContext.Request.Headers["User-Agent"],
                    Timestamp = DateTime.UtcNow,
                    TenantId = _tenantContext.TenantId
                };
                
                context.Set<AuditLog>().Add(audit);
            }
        }
        
        return base.SavingChangesAsync(eventData, result, ct);
    }
}
```

## Security Best Practices

### 1. Never Expose Sensitive Data

```csharp
public class UserDto
{
    public Ulid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    // ❌ NEVER include password hash in DTOs
    // public string PasswordHash { get; set; }
}
```

### 2. Use Environment Variables

```bash
# ❌ Bad - hardcoded secrets
JWT_SECRET_KEY=my-secret-key

# ✅ Good - use secret management
JWT_SECRET_KEY=${VAULT_JWT_SECRET}
```

### 3. Validate All Inputs

```csharp
// ✅ Always validate
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}
```

### 4. Use Parameterized Queries

```csharp
// ✅ Good - parameterized
var user = await _context.Users
    .FirstOrDefaultAsync(u => u.Email == email);

// ❌ Bad - SQL injection risk
var sql = $"SELECT * FROM Users WHERE Email = '{email}'";
```

### 5. Implement Rate Limiting

```csharp
[EnableRateLimiting("fixed")]
[HttpPost("api/auth/login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    // Protected endpoint
}
```

### 6. Use HTTPS Only in Production

```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
```

### 7. Keep Dependencies Updated

```bash
# Check for vulnerabilities
dotnet list package --vulnerable

# Update packages
dotnet add package <PackageName> --version <LatestVersion>
```

## Production Checklist

- [ ] Change all default passwords
- [ ] Generate strong JWT secret keys
- [ ] Enable HTTPS/TLS
- [ ] Configure CORS properly
- [ ] Enable rate limiting
- [ ] Set up audit logging
- [ ] Configure security headers
- [ ] Enable request size limits
- [ ] Set up monitoring and alerts
- [ ] Review and test authorization policies
- [ ] Implement password policies
- [ ] Set up account lockout
- [ ] Enable two-factor authentication (if needed)
- [ ] Configure backup and disaster recovery
- [ ] Perform security audit
- [ ] Document security procedures

## Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [OAuth 2.0](https://oauth.net/2/)
- [OpenID Connect](https://openid.net/connect/)
