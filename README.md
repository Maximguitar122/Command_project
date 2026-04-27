# Luftreise - Веб-додаток для бронювання авіаквитків

## Опис проєкту

Luftreise - це сучасна онлайн-платформа для пошуку, бронювання та купівлі авіаквитків, розроблена командою "Бригантина" як курсовий проєкт IT STEP Academy. 

---

## Команда розробки

- **Загоруйко Олександр Дмитрович** — Product Owner
- **Качуровський Руслан Русланович** — Project Manager / Team Lead, Full Stack Developer, Technical Writer
- **Скрильников Микита Сергійович** — Co-Team Lead, Backend Developer
- **Залуга Максим Петрович** — Key Frontend Developer, UI/UX Designer
- **Корсун Микола Денисович** — Primary Backend Developer
- **Циміданов Герман Андрійович** — QA Engineer, Backend Developer

---

## Технологічний стек

### Backend
- ASP.NET Core 10.0
- Entity Framework Core 10.0
- NetTopologySuite (підтримка геоданих)
- Repository Pattern + Clean Architecture
- SQL Server

### Frontend
- ASP.NET Core MVC + Razor Views
- Bootstrap 5
- HTML5, CSS3, JavaScript (jQuery + Validation)

### Архітектура
- **Clean Architecture** (Domain → Application → Infrastructure → Web)
- CQRS (MediatR — частково)
- Dependency Injection
- Repository Pattern

### Тестування
- NUnit

## Структура проєкту

```
Luftreise/
├── src/
│   ├── Luftreise.Domain/              # Entities, Enums, Value Objects
│   ├── Luftreise.Application/         # DTOs, Commands, Queries, Interfaces
│   ├── Luftreise.Infrastructure/      # DbContext, Repositories, EF Core
├── Luftreise(Command_project).csproj  # Controllers, Views, wwwroot
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
- .NET 10.0 SDK
- SQL Server (LocalDB або повноцінний сервер)
- Visual Studio 2022 / Rider / VS Code

### Кроки встановлення

1. **Клонуйте репозиторій** та перейдіть у папку проєкту:

```bash
cd "D:\IT STEP\!!!\Командний проєкт №1\Command_project"
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
```

4. Створіть міграції та базу даних:
```bash
dotnet ef migrations add InitialCreate `
  --project src/Luftreise.Infrastructure `
  --startup-project "Luftreise(Command_project).csproj"

dotnet ef database update `
  --project src/Luftreise.Infrastructure `
  --startup-project "Luftreise(Command_project).csproj"
```

5. Запустіть додаток:
```bash
dotnet run --project "Luftreise(Command_project).csproj"
```
Або через Visual Studio — просто натисніть F5.
Відкрийте браузер: `https://localhost:7171` або `http://localhost:5238`


## Запуск тестів

```bash
cd tests/Luftreise.Tests
dotnet test
```

## Робота з міграціями

```bash
# Створити нову міграцію
dotnet ef migrations add <MigrationName> `
  --project src/Luftreise.Infrastructure `
  --startup-project "Luftreise(Command_project).csproj"

# Застосувати міграції
dotnet ef database update `
  --project src/Luftreise.Infrastructure `
  --startup-project "Luftreise(Command_project).csproj"
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
