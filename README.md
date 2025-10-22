# FIFA Tracker

Aplikacja do śledzenia statystyk meczów FIFA granych ze znajomymi.

## 🎮 Funkcjonalności

- Zarządzanie użytkownikami (soft delete - historia meczów zachowana)
- Sesje gier (1v1, 2v2, 2v1) z automatycznym generowaniem meczów
- Tworzenie customowych meczów
- Leaderboard ze statystykami
- Responsywny UI z modals i hamburger menu

## 🚀 Szybki start

### Lokalne uruchomienie (Docker)

```bash
git clone https://github.com/minuss01/FifaTracker.git
cd FifaTracker
docker-compose up -d
```

- **Frontend:** http://localhost:3000
- **API:** http://localhost:5000
- **Swagger:** http://localhost:5000/swagger

### Produkcja

Zobacz: [QUICK_START_PRODUCTION.md](./QUICK_START_PRODUCTION.md)

## 💻 Stack

- **Backend:** .NET 9, Clean Architecture, CQRS (MediatR), EF Core, PostgreSQL
- **Frontend:** React 19, TypeScript, Vite
- **Infrastructure:** Docker, Cloudflare Tunnel

## 📱 Dostęp z telefonu (sieć lokalna)

```powershell
# Lub ręcznie:
# 1. Znajdź IP: ipconfig
# 2. Utwórz frontend/.env: VITE_API_BASE_URL=http://192.168.1.X:5000/api
# 3. Restart: docker-compose up --build -d
# 4. Otwórz: http://192.168.1.X:3000 na telefonie
```

## 🔧 Zmienne środowiskowe

**Backend (PostgreSQL):**
```env
POSTGRES_HOST=postgres           # localhost lub IP zewnętrznej bazy
POSTGRES_PORT=5432
POSTGRES_DB=fifatracker
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DATA_PATH=/path/to/data # Opcjonalnie: własna ścieżka dla danych
```

**Frontend:**
```env
VITE_API_BASE_URL=http://localhost:5000/api
```

**CORS (produkcja):**
```env
ALLOWED_ORIGINS=https://twoja-domena.com
```
## 📝 Licencja

MIT
