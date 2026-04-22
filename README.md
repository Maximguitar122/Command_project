# Luftreise - Веб-додаток для бронювання авіаквитків

## Опис проєкту

Luftreise - це сучасна онлайн-платформа для пошуку, бронювання та купівлі авіаквитків, розроблена командою "Бригантина" як курсовий проєкт IT STEP Academy.

## Команда розробки

- **Загоруйко Олександр Дмитрович** — Product Owner
- **Качуровський Руслан Русланович** — Project Manager / Team Lead, Full Stack Developer, Technical Writer
- **Залуга Максим Петрович** — Frontend Developer, QA Engineer
- **Корсун Микола Денисович** — Backend Developer

## Технологічний стек

### Backend
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- Dapper
- MediatR (CQRS pattern)
- SQL Server з підтримкою геоданих (NetTopologySuite)

### Frontend
- ASP.NET Core MVC
- Razor Views
- Bootstrap 5
- HTML5, CSS3, JavaScript

### Архітектура
- Clean Architecture (Domain, Application, Infrastructure, Web)
- Repository Pattern
- CQRS з MediatR
- Dependency Injection

### Тестування
- NUnit

## Структура проєкту

```
Luftreise/
├── src/
│   ├── Luftreise.Domain/          # Entities, Enums, Value Objects
│   ├── Luftreise.Application/     # DTOs, Commands, Queries, Interfaces
│   ├── Luftreise.Infrastructure/  # DbContext, Repositories, EF Core
│   └── Luftreise.Web/             # Controllers, Views, wwwroot
├── tests/
│   └── Luftreise.Tests/           # NUnit тести
└── Luftreise.sln
```

## Основні функції

- ✈️ Пошук авіарейсів за містом, датою та кількістю пасажирів
- 📋 Бронювання квитків онлайн
- 👤 Управління профілем користувача
- 📊 Адміністративна панель
- 🌍 Підтримка геоданих для аеропортів
- 📱 Responsive design для мобільних пристроїв

## Встановлення та запуск

### Вимоги
- .NET 9.0 SDK
- SQL Server або SQL Server LocalDB
- Visual Studio 2022 / VS Code / Rider

### Кроки встановлення

1. Клонуйте репозиторій:
```bash
git clone <repository-url>
cd 777
```

2. Відновіть NuGet пакети:
```bash
dotnet restore
```

3. Оновіть connection string у файлі `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LuftreiseDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}

4. Створіть міграції та базу даних:
```bash
dotnet ef migrations add InitialCreate --project .\Luftreise.Infrastructure.csproj --startup-project "..\..\Luftreise(Command_project).csproj"
dotnet ef database update --project .\Luftreise.Infrastructure.csproj --startup-project "..\..\Luftreise(Command_project).csproj"
```

5. Запустіть додаток:
```bash
cd ../Luftreise.Web
dotnet run
```

6. Відкрийте браузер: `https://localhost:5001`

## Запуск тестів

```bash
cd tests/Luftreise.Tests
dotnet test
```

## Розробка

### Додавання нової міграції
```bash
cd src/Luftreise.Infrastructure
dotnet ef migrations add <MigrationName> --startup-project ../Luftreise.Web
dotnet ef database update --startup-project ../Luftreise.Web
```

### Збірка проєкту
```bash
dotnet build
```

### Публікація
```bash
dotnet publish -c Release -o ./publish
```

## Патерни та принципи

- **SOLID principles**
- **Clean Architecture** - розділення на шари з чіткими залежностями
- **CQRS** - розділення команд та запитів через MediatR
- **Repository Pattern** - абстракція доступу до даних
- **Dependency Injection** - слабке зв'язування компонентів
- **Async/Await** - асинхронне програмування

## Майбутні покращення

- 🔐 JWT Authentication & Authorization
- 💳 Інтеграція платіжних систем
- 📧 Email notifications
- 🔔 SignalR для real-time оновлень
- ⭐ Система рейтингів та відгуків
- 🎁 Програма лояльності
- 🌐 Мультимовність (EN, UA, DE)
- 📱 Blazor компоненти для інтерактивності

## Ліцензія

Курсовий проєкт IT STEP Academy © 2026

## Контакти

- Замовник: IT STEP Academy, Одеська філія
- Керівник проєкту: Олександр Загоруйко
