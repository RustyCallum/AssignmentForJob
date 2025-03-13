# API do Zarządzania Zadaniami

## Opis projektu

API umożliwia zarządzanie zadaniami poprzez operacje CRUD (tworzenie, pobieranie, aktualizowanie i usuwanie). Dodatkowo, aplikacja obsługuje automatyczne wysyłanie przypomnień e-mail dla zadań, których termin realizacji upływa w ciągu określonej liczby godzin. Przypomnienia są wysyłane za pomocą mechanizmu Hangfire.

## Struktura projektu

```
ForJob/
│-- Controllers/       # Kontrolery API
│-- Domain/            # Definicje modeli danych
│-- Services/          # Logika biznesowa i obsługa e-maili
│-- DbContext/         # Kontekst bazy danych
│-- Migrations/        # Migracje bazy danych
│-- appsettings.json   # Plik konfiguracyjny
│-- Program.cs         # Główny plik uruchomieniowy aplikacji
```

## Konfiguracja

Plik `appsettings.json` zawiera wszystkie niezbędne ustawienia:

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "Username": "YOURUSERNAME",
    "Password": "YOURPASSWORD"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=forjob.db",
    "HangfireConnection": "Data Source=forjob.db;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Hangfire": "Information"
    }
  },
  "AllowedHosts": "*",
  "RecipentEmail": "PROVIDERECIPENTEMAILHERE",
  "Hangfire": {
    "TaskReminderCron": "*/5 * * * *"
  }
}
```

## Uruchomienie projektu

1. Przygotuj środowisko uruchomieniowe .NET.
2. Sklonuj repozytorium i przejdź do katalogu projektu.
3. Zainstaluj zależności:
   ```sh
   dotnet restore
   ```
4. Uruchom migracje bazy danych:
   ```sh
   dotnet ef database update
   ```
5. Uruchom aplikację:
   ```sh
   dotnet run
   ```

## Dokumentacja API

Do testowania API można użyć Postmana. W repozytorium znajduje się plik kolekcji Postman (`ForJob.postman_collection.json`), który zawiera gotowe zapytania do przetestowania endpointów.

## Endpointy API

### Tworzenie zadania

- **POST** `/api/tasks`
- Body (JSON):
  ```json
  {
    "title": "Przykładowe zadanie",
    "description": "Opis zadania",
    "dueDate": "2025-03-13T15:00:00Z"
  }
  ```

### Pobieranie wszystkich zadań, które nie są oznaczone jako usunięte

- **GET** `/api/tasks`

### Pobieranie pojedynczego zadania, ignorowany jest status usunięcia

- **GET** `/api/tasks/{id}`

### Aktualizowanie zadania

- **PUT** `/api/tasks/{id}`
- Body (JSON):
  ```json
  {
    "title": "Zaktualizowane zadanie",
    "description": "Nowy opis",
    "dueDate": "2025-03-14T12:00:00Z"
  }
  ```

### Usuwanie zadania

- **DELETE** `/api/tasks/{id}`

### Dodanie użytkownika

- **POST** `/api/user`

### Logowanie

- **POST** `/api/user/login`

## Mechanizm przypomnień o zadaniach

Hangfire jest skonfigurowany do uruchamiania zadania sprawdzającego terminy realizacji co 5 minut. Jeśli termin realizacji zadania upływa w ciągu 6 godzin, system wysyła e-mail przypominający na adres skonfigurowany w `RecipentEmail`.

## Autor

Aleksander Duda

