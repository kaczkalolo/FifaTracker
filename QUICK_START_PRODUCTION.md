# 🚀 FIFA Tracker - Deployment Produkcyjny (Cloudflare)

## Wymagania

- Serwer z Docker i Docker Compose
- Konto Cloudflare (darmowe)
- Domena (może być subdomena z Cloudflare)

---

## 1️⃣ Przygotowanie serwera

```bash
git clone https://github.com/minuss01/FifaTracker.git
cd FifaTracker
cp .env.production.example .env.production
```

Edytuj `.env.production`:
```env
POSTGRES_PASSWORD=TwojeMocneHaslo123!
VITE_API_BASE_URL=/api
ALLOWED_ORIGINS=https://twojadomena.com
```

**Gotowe!** Nginx obsłuży routing - frontend i API na tej samej domenie.

---

## 3️⃣ Uruchomienie

```bash
# Uruchom kontenery
docker-compose -f docker-compose.prod.yml --env-file .env.production up -d --build

# Uruchom tunnel jako service
cloudflared service install
# Linux: sudo systemctl start cloudflared && sudo systemctl enable cloudflared
```

---

## 🆘 Troubleshooting

**CORS errors:**
```bash
# Sprawdź appsettings.Production.json - AllowedOrigins
docker logs fifatracker-api
```

**API nie odpowiada:**
```bash
docker logs fifatracker-api
curl http://localhost:5000/api/users
```

---

## 💾 Backup bazy danych

```bash
# Backup
docker exec fifatracker-postgres pg_dump -U postgres fifatracker > backup_$(date +%Y%m%d).sql

# Restore
cat backup_20251020.sql | docker exec -i fifatracker-postgres psql -U postgres fifatracker
```

---

## 🔄 Aktualizacja aplikacji

```bash
git pull
docker-compose -f docker-compose.prod.yml --env-file .env.production up -d --build
```

**Dane są bezpieczne!** 
- Domyślnie: baza w Docker volume `postgres_data` - przetrwa restart i rebuild
- Opcjonalnie: ustaw `POSTGRES_DATA_PATH` w `.env.production` aby dane były w konkretnym folderze
- Volume nie jest usuwany podczas `docker-compose down`
- Volume jest usuwany tylko przez `docker-compose down -v` (NIE rob tego!)

---

## 🗄️ Zewnętrzna baza danych

### Jeśli jest już baza PostgreSQL:

**1. W `.env.production` ustaw:**
```env
POSTGRES_HOST=192.168.1.100      # IP serwera bazy
POSTGRES_PORT=5432
POSTGRES_DB=fifatracker
POSTGRES_USER=postgres
POSTGRES_PASSWORD=HasłoDoBazy123!
```

**2. W `docker-compose.prod.yml` zakomentuj sekcję `postgres`:**
```yaml
  # postgres:  # <- Zakomentuj cały serwis postgres
  #   image: postgres:16-alpine
  #   ...
```

**3. Usuń `depends_on` w sekcji `api`:**
```yaml
  api:
    ...
    # depends_on:  # <- Zakomentuj to
    #   postgres:
    #     condition: service_healthy
```

**4. Uruchom tylko API i Frontend:**
```bash
docker-compose -f docker-compose.prod.yml --env-file .env.production up -d api frontend
```

Migracje zostaną automatycznie zastosowane przy starcie API! ✅

---

## 💾 Własna ścieżka dla danych PostgreSQL

### Jeśli chcesz mieć dane w konkretnym folderze (łatwiejsze backupy):

**1. W `.env.production` ustaw:**
```env
POSTGRES_DATA_PATH=/home/user/fifatracker-data
```

**Windows:**
```env
POSTGRES_DATA_PATH=C:/FifaTracker/data
```

**2. Utwórz folder (jeśli nie istnieje):**
```bash
# Linux/Mac
mkdir -p /home/user/fifatracker-data
chmod 777 /home/user/fifatracker-data  # PostgreSQL potrzebuje praw zapisu

# Windows
mkdir C:\FifaTracker\data
```

**3. Uruchom normalnie:**
```bash
docker-compose -f docker-compose.prod.yml --env-file .env.production up -d
```

**Zalety:**
- ✅ Łatwy dostęp do plików bazy (do backupów)
- ✅ Możesz przenieść folder na inny dysk
- ✅ Backupy przez zwykłe kopiowanie folderu
- ✅ Widoczne w systemie plików (nie ukryte w Docker volumes)
