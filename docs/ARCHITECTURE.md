# Luftreise - Структура проєкту

## Огляд архітектури

Проєкт побудований за принципами Clean Architecture з чітким розділенням відповідальності між шарами.

```
┌─────────────────────────────────────────┐
│           Luftreise.Web                 │
│    (Controllers, Views, UI Logic)       │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│      Luftreise.Infrastructure           │
│  (DbContext, Repositories, EF Core)     │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│       Luftreise.Application             │
│   (CQRS, DTOs, Business Logic)          │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│         Luftreise.Domain                │
│      (Entities, Value Objects)          │
└─────────────────────────────────────────┘
```

## Шари проєкту

### 1. Domain Layer (Luftreise.Domain)
**Призначення**: Ядро бізнес-логіки, незалежне від зовнішніх залежностей.

**Містить**:
- `Entities/` - доменні сутності (User, Flight, Booking, Airport, Passenger)
- `Enums/` - перерахування (FlightStatus, BookingStatus, UserRole)

**Залежності**: Немає (чистий C#)

### 2. Application Layer (Luftreise.Application)
**Призначення**: Оркестрація бізнес-логіки, CQRS patterns.

**Містить**:
- `Commands/` - команди для зміни стану (CreateBookingCommand)
- `Queries/` - запити для читання даних (SearchFlightsQuery)
- `DTOs/` - об'єкти передачі даних
- `Interfaces/` - контракти для repositories

**Залежності**: Domain, MediatR

**Патерни**: CQRS, Mediator

### 3. Infrastructure Layer (Luftreise.Infrastructure)
**Призначення**: Реалізація доступу до даних та зовнішніх сервісів.

**Містить**:
- `Data/` - DbContext, конфігурація EF Core
- `Repositories/` - реалізація repository pattern
- `DependencyInjection.cs` - реєстрація сервісів

**Залежності**: Domain, Application, EF Core, Dapper

**Технології**: 
- Entity Framework Core 9.0
- SQL Server з NetTopologySuite для геоданих
- Dapper для оптимізованих запитів

### 4. Web Layer (Luftreise.Web)
**Призначення**: Презентаційний шар, взаємодія з користувачем.

**Містить**:
- `Controllers/` - MVC контролери
- `Views/` - Razor views
- `wwwroot/` - статичні файли

**Залежності**: Application, Infrastructure

## Ключові патерни

### CQRS (Command Query Responsibility Segregation)
Розділення операцій читання та запису:
- **Commands** - змінюють стан системи (CreateBookingCommand)
- **Queries** - читають дані без змін (SearchFlightsQuery)

### Repository Pattern
Абстракція доступу до даних:
```csharp
IFlightRepository -> FlightRepository (EF Core)
IBookingRepository -> BookingRepository (EF Core)
```

### Dependency Injection
Всі залежності впроваджуються через конструктори, реєстрація в `Program.cs`.

## Потік даних

### Пошук рейсів
1. User → FlightsController.Search()
2. Controller → MediatR.Send(SearchFlightsQuery)
3. MediatR → SearchFlightsQueryHandler
4. Handler → IFlightRepository.SearchFlightsAsync()
5. Repository → EF Core → SQL Server
6. Результат → FlightDto[] → View

### Створення бронювання
1. User → BookingsController.Create()
2. Controller → MediatR.Send(CreateBookingCommand)
3. MediatR → CreateBookingCommandHandler
4. Handler → IBookingRepository + IFlightRepository
5. Транзакція → SQL Server
6. Результат → BookingDto → Confirmation View

## База даних

### Основні таблиці
- **Users** - користувачі системи
- **Airports** - аеропорти з геоданими
- **Flights** - авіарейси
- **Bookings** - бронювання
- **Passengers** - пасажири

### Зв'язки
- Flight → Airport (DepartureAirport, ArrivalAirport)
- Booking → Flight (many-to-one)
- Booking → User (many-to-one)
- Passenger → Booking (many-to-one)

## Тестування

### Unit Tests (Luftreise.Tests)
- NUnit framework
- Тестування entities, repositories, handlers
- In-memory database для інтеграційних тестів

## Розширення проєкту

### Додавання нової функції

1. **Domain**: Створити entity/enum якщо потрібно
2. **Application**: 
   - Додати DTO
   - Створити Command/Query
   - Реалізувати Handler
3. **Infrastructure**: Додати repository якщо потрібно
4. **Web**: Створити Controller та Views

### Приклад: Додавання відгуків

```
Domain: Review entity
Application: CreateReviewCommand, GetReviewsQuery
Infrastructure: ReviewRepository
Web: ReviewsController, Review views
```

## Конфігурація

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=LuftreiseDb;..."
  }
}
```

### Program.cs
```csharp
builder.Services.AddInfrastructure(configuration);
builder.Services.AddMediatR(...);
```

## Безпека

- Аутентифікація: ASP.NET Core Identity (планується)
- Авторизація: Role-based (User, Admin)
- SQL Injection: Захист через EF Core параметризовані запити
- XSS: Razor автоматично екранує вивід

## Продуктивність

- Async/await для всіх I/O операцій
- EF Core Include() для eager loading
- Dapper для складних read-only запитів
- Кешування (планується)

## Логування

- ASP.NET Core logging framework
- Рівні: Information, Warning, Error
- Вивід: Console, File (налаштовується)
