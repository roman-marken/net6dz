# Spanish Football Championship

Практичне завдання з теми **Entity Framework Core** для проєкту
«Чемпіонат Іспанії з футболу».

Проєкт містить:

- моделі EF Core для команд, гравців, матчів і голів;
- подання для статистики бомбардирів, голів команд і турнірних очок;
- збережену процедуру для випадкового заповнення матчів;
- сервіс із методами для завдань 1-4;
- xUnit тести для коду з EF Core.

## Структура

```text
src/SpanishFootballChampionship
  Data/
  Models/
  Services/
  Sql/
tests/SpanishFootballChampionship.Tests
```

## Запуск

1. Встановіть .NET SDK 8.
2. Створіть базу SQL Server.
3. У файлі `appsettings.json` змініть connection string.
4. Запустіть:

```bash
dotnet restore
dotnet run --project src/SpanishFootballChampionship
```

## Тести

```bash
dotnet test
```

У тестах використовується SQLite in-memory база, щоб перевірити EF Core запити
без окремого SQL Server.
