# 🏗️ Kiến trúc hệ thống

## Tổng quan

Dự án sử dụng **Clean Architecture** kết hợp với **Domain-Driven Design (DDD)** để xây dựng một hệ thống microservice có khả năng mở rộng và bảo trì cao.

## Nguyên tắc thiết kế

### 1. Separation of Concerns
Mỗi layer có trách nhiệm riêng biệt và không phụ thuộc vào implementation details của layer khác.

### 2. Dependency Rule
Dependencies chỉ được phép point inward. Code trong layer bên trong không được biết gì về layer bên ngoài.

```
Domain (Core) ← Application ← Infrastructure
                            ← API
```

### 3. Testability
Mọi business logic đều có thể test độc lập mà không cần database hay external services.

## Layer Architecture

### 🎯 Domain Layer
**Trách nhiệm**: Chứa business logic và domain entities

**Bao gồm**:
- Entities & Aggregate Roots
- Value Objects
- Domain Events
- Domain Exceptions
- Domain Services (nếu cần)

**Đặc điểm**:
- Không phụ thuộc vào bất kỳ layer nào khác
- Không có framework dependencies
- Chỉ chứa pure business logic

**Ví dụ**:
```
Domain/
├── Entities/
│   ├── User.cs
│   ├── Role.cs
│   └── Permission.cs
├── ValueObjects/
│   ├── Email.cs
│   └── PhoneNumber.cs
├── Events/
│   └── UserCreatedEvent.cs
└── Exceptions/
    └── DomainException.cs
```

### 🔄 Application Layer
**Trách nhiệm**: Orchestrate business workflows và use cases

**Bao gồm**:
- Commands & Queries (CQRS)
- Command/Query Handlers
- DTOs (Data Transfer Objects)
- Application Services
- Interfaces (Repository, External Services)
- Behaviors (Validation, Logging, Transaction)

**Đặc điểm**:
- Phụ thuộc vào Domain Layer
- Định nghĩa interfaces cho Infrastructure
- Không biết về implementation details

**Ví dụ**:
```
Application/
├── Commands/
│   ├── CreateUser/
│   │   ├── CreateUserCommand.cs
│   │   ├── CreateUserCommandHandler.cs
│   │   └── CreateUserCommandValidator.cs
├── Queries/
│   ├── GetUser/
│   │   ├── GetUserQuery.cs
│   │   └── GetUserQueryHandler.cs
├── Interfaces/
│   ├── IUserRepository.cs
│   └── IEmailService.cs
└── Behaviors/
    ├── ValidationBehavior.cs
    └── LoggingBehavior.cs
```

### 🗄️ Infrastructure Layer
**Trách nhiệm**: Implement interfaces được định nghĩa trong Application Layer

**Bao gồm**:
- Repository Implementations
- Database Context (EF Core)
- External Service Clients
- File System Access
- Email Services
- Cache Services

**Đặc điểm**:
- Implement interfaces từ Application Layer
- Chứa tất cả technical details
- Có thể thay đổi mà không ảnh hưởng Domain/Application

**Ví dụ**:
```
Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Repositories/
│   │   └── UserRepository.cs
│   └── Configurations/
│       └── UserConfiguration.cs
├── Services/
│   ├── EmailService.cs
│   └── CacheService.cs
└── UnitOfWorks/
    └── UnitOfWork.cs
```

### 🌐 API Layer
**Trách nhiệm**: HTTP endpoints và API configuration

**Bao gồm**:
- Controllers/Endpoints
- Middleware
- Filters
- API Configuration
- Dependency Injection Setup

**Đặc điểm**:
- Entry point của application
- Handle HTTP requests/responses
- Thin layer - chỉ delegate to Application Layer

**Ví dụ**:
```
Api/
├── Controllers/
│   └── UsersController.cs
├── Middlewares/
│   ├── ExceptionMiddleware.cs
│   └── TenantMiddleware.cs
├── Filters/
│   └── ValidationFilter.cs
└── Program.cs
```

## Shared Libraries

### 📦 Libs Structure

Các shared libraries giúp tái sử dụng code giữa các microservices:

```
libs/
├── Api/                    # API base configuration
├── Application/            # Application patterns
├── Contracts/              # Shared DTOs & contracts
├── Domain/                 # Domain base classes
├── DynamicQuery/          # Query builder
├── FluentConfiguration/   # Elasticsearch config
├── Infrastructure/        # Data access base
├── SharedKernel/         # Common utilities
└── Specification/        # Specification pattern
```

### Api Library
**Purpose**: Shared API configuration và middleware

**Features**:
- API Versioning setup
- Swagger/OpenAPI configuration
- Health checks
- Problem Details
- OpenTelemetry instrumentation
- Validation filters

### Application Library
**Purpose**: CQRS patterns và behaviors

**Features**:
- MediatR pipeline behaviors
- Validation behavior
- Logging behavior
- Transaction behavior
- Query string processing

### Contracts Library
**Purpose**: Shared DTOs và API contracts

**Features**:
- Request/Response DTOs
- API wrapper models
- Constants
- Binding models

### Domain Library
**Purpose**: Base domain classes

**Features**:
- BaseEntity
- AggregateRoot
- Value converters (ULID)

### DynamicQuery Library
**Purpose**: Dynamic query building

**Features**:
- Dynamic filtering
- Dynamic sorting
- Dynamic pagination
- Query string parsing

### Infrastructure Library
**Purpose**: Base repository và Unit of Work

**Features**:
- Generic repository pattern
- Unit of Work pattern
- Database interceptors
- Audit fields handling

### SharedKernel Library
**Purpose**: Common utilities

**Features**:
- Common exceptions
- Extension methods
- Domain events
- Tenant management
- Base models

### Specification Library
**Purpose**: Specification pattern implementation

**Features**:
- Specification builder
- Include expressions
- Query evaluators
- Complex query composition

## Patterns & Practices

### CQRS (Command Query Responsibility Segregation)

**Commands**: Thay đổi state của system
```csharp
public class CreateUserCommand : IRequest<Result<UserDto>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken ct)
    {
        // Create user logic
    }
}
```

**Queries**: Đọc data từ system
```csharp
public class GetUserQuery : IRequest<Result<UserDto>>
{
    public Ulid UserId { get; set; }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken ct)
    {
        // Get user logic
    }
}
```

### Repository Pattern

```csharp
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(Ulid userId);
}

public class UserRepository : Repository<User>, IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}
```

### Specification Pattern

```csharp
public class ActiveUsersSpecification : Specification<User>
{
    public ActiveUsersSpecification()
    {
        Query.Where(u => u.IsActive == true);
    }
}

// Usage
var spec = new ActiveUsersSpecification();
var users = await _repository.ListAsync(spec);
```

### Unit of Work Pattern

```csharp
public interface IUnitOfWork
{
    IUserRepository Users { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

// Usage in handler
await _unitOfWork.BeginTransactionAsync();
try
{
    await _unitOfWork.Users.AddAsync(user);
    await _unitOfWork.SaveChangesAsync();
    await _unitOfWork.CommitTransactionAsync();
}
catch
{
    await _unitOfWork.RollbackTransactionAsync();
    throw;
}
```

### Domain Events

```csharp
public class UserCreatedEvent : DomainEvent
{
    public Ulid UserId { get; }
    public string Email { get; }
    
    public UserCreatedEvent(Ulid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}

// In Entity
public void Create()
{
    // Business logic
    AddDomainEvent(new UserCreatedEvent(Id, Email));
}

// Event Handler
public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken ct)
    {
        // Send welcome email, etc.
    }
}
```

## Multi-tenancy

Hệ thống hỗ trợ multi-tenancy với các tính năng:

### Tenant Context
```csharp
public interface ITenantContext
{
    string? TenantId { get; }
    bool HasTenant { get; }
}
```

### Tenant Middleware
Tự động extract tenant từ:
- Header: `X-Tenant-Id`
- Query string: `?tenantId=xxx`
- JWT Claims

### Query Filtering
Tự động filter data theo tenant:
```csharp
public class TenantQueryFilter : IQueryFilter
{
    public void Apply(EntityTypeBuilder builder, ITenantContext context)
    {
        builder.HasQueryFilter(e => 
            !context.HasTenant || e.TenantId == context.TenantId);
    }
}
```

## Validation Strategy

### 1. Input Validation (FluentValidation)
```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).MinimumLength(8);
    }
}
```

### 2. Domain Validation (Domain Layer)
```csharp
public class User : AggregateRoot
{
    public void ChangeEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new DomainException("Email cannot be empty");
            
        Email = newEmail;
    }
}
```

### 3. Validation Pipeline Behavior
```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, ...)
    {
        var validationResults = await ValidateAsync(request);
        if (validationResults.Any())
            throw new ValidationException(validationResults);
            
        return await next();
    }
}
```

## Error Handling

### Application Errors
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error Error { get; }
    
    public static Result<T> Success(T value);
    public static Result<T> Failure(Error error);
}
```

### Exception Middleware
```csharp
public class ExceptionMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            await HandleDomainException(context, ex);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnknownException(context, ex);
        }
    }
}
```

## Observability

### Logging (Serilog)
- Structured logging
- Log enrichment (Tenant, User, Correlation Id)
- Sink to Seq

### Tracing (OpenTelemetry)
- Distributed tracing
- Automatic instrumentation
- Custom spans
- Export to Jaeger

### Metrics
- Health checks
- Performance counters
- Custom metrics

## Testing Strategy

### Unit Tests
Test business logic trong Domain và Application layers:
```csharp
[Fact]
public async Task CreateUser_WithValidData_ShouldSucceed()
{
    // Arrange
    var command = new CreateUserCommand { ... };
    var handler = new CreateUserCommandHandler(...);
    
    // Act
    var result = await handler.Handle(command, default);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
}
```

### Integration Tests
Test toàn bộ flow từ API đến Database:
```csharp
[Fact]
public async Task POST_CreateUser_ReturnsCreated()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new { ... };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/users", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
}
```

## Best Practices

### 1. Keep Domain Pure
- No framework dependencies trong Domain Layer
- Business logic thuộc về Domain, không phải Application

### 2. Use Value Objects
- Email, PhoneNumber, Money, Address nên là Value Objects
- Immutable và self-validating

### 3. Keep Controllers Thin
- Controller chỉ nên delegate to MediatR
- Không có business logic trong Controller

### 4. Use Async/Await Properly
- Avoid async void (trừ event handlers)
- Use ConfigureAwait(false) trong libraries
- Always pass CancellationToken

### 5. Follow SOLID Principles
- Single Responsibility
- Open/Closed
- Liskov Substitution
- Interface Segregation
- Dependency Inversion

## Deployment Architecture

```
┌─────────────┐
│   Nginx     │ (API Gateway, Load Balancer)
│   :80       │
└──────┬──────┘
       │
┌──────┴────────────────────────────────┐
│                                       │
│  ┌───────────────┐  ┌──────────────┐ │
│  │   Identity    │  │   Other      │ │
│  │   Service     │  │   Services   │ │
│  │   :5001       │  │   :5xxx      │ │
│  └───────┬───────┘  └──────┬───────┘ │
│          │                 │         │
└──────────┼─────────────────┼─────────┘
           │                 │
    ┌──────┴─────────────────┴──────┐
    │                                │
┌───▼───────┐  ┌──────────┐  ┌─────▼────┐
│ PostgreSQL│  │  Redis   │  │ RabbitMQ │
│   :5432   │  │  :6379   │  │  :5672   │
└───────────┘  └──────────┘  └──────────┘
    │
┌───▼────────────────────────────────┐
│ Monitoring & Observability         │
│ - Seq (Logs)                       │
│ - Jaeger (Tracing)                 │
│ - Health Checks                    │
└────────────────────────────────────┘
```

## Security Considerations

### 1. Authentication & Authorization
- JWT with refresh tokens
- Role-based access control
- Claims-based authorization

### 2. Data Protection
- Encrypt sensitive data at rest
- Use HTTPS in production
- Secure connection strings

### 3. Input Validation
- Validate all inputs
- Sanitize user data
- Use parameterized queries

### 4. API Security
- Rate limiting
- CORS configuration
- API versioning

## Scalability

### Horizontal Scaling
- Stateless services
- Externalize session (Redis)
- Database connection pooling

### Vertical Scaling
- Resource optimization
- Connection pooling
- Caching strategy

### Performance
- Use async/await
- Lazy loading vs Eager loading
- Pagination for large datasets
- Response caching

## Future Enhancements

- [ ] Event Sourcing
- [ ] SAGA Pattern for distributed transactions
- [ ] API Gateway (Ocelot/YARP)
- [ ] gRPC for inter-service communication
- [ ] GraphQL endpoint
- [ ] Background jobs (Hangfire)
- [ ] Real-time notifications (SignalR)
- [ ] Message outbox pattern
