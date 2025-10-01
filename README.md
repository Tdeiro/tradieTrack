# TradieTrack

Full-stack app for tradies (jobs, quotes, invoices).  
**Backend:** .NET 9 (Minimal APIs) • **Frontend:** React + TypeScript (Vite) • **UI:** Tailwind CSS v4 • **DB:** Postgres (Docker)

---

## 📑 Table of contents
- [Prerequisites](#prerequisites)  
- [Project structure](#project-structure)  
- [Getting started (quick)](#getting-started-quick)  
- [Backend (API)](#backend-api)  
- [Frontend (Web)](#frontend-web)  
- [Database (Postgres via Docker)](#database-postgres-via-docker)  
- [Environment variables](#environment-variables)  
- [OpenAPI / API docs](#openapi--api-docs)  
- [CORS](#cors)  
- [Scripts](#scripts)  
- [Troubleshooting](#troubleshooting)  
- [Next steps](#next-steps)  
- [License](#license)

---

## 🔧 Prerequisites
- **.NET SDK 9.x** → `dotnet --version`  
- **Node.js 18+** & npm → `node -v`, `npm -v`  
- **Docker Desktop** (for Postgres) → `docker --version`  
- **Git** → `git --version`

Optional:  
- VS Code with extensions: *C# Dev Kit*, *Tailwind CSS IntelliSense*, *ESLint*

---

## 📂 Project structure

- TradieTrack/
- tradieTrack.sln
- TradieTrack.Api/ # .NET 9 Minimal API
- TradieTrack.Web/ # React + TS (Vite)
- docker-compose.yml # Postgres + pgAdmin (dev)
- README.md
- .gitignore

---

## 🚀 Getting started (quick)

1. **Start database (dev)**  
   ```bash
   docker-compose up -d
2. **Run API**
    cd TradieTrack.Api
    dotnet run
    # Example: http://localhost:5099
3. **Run Web**
    cd ../TradieTrack.Web
    npm i
    npm run dev
    # Opens http://localhost:5173