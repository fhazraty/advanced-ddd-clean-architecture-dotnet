# Advanced DDD Clean Architecture .NET

An advanced Domain-Driven Design (DDD) sample built with .NET demonstrating Clean Architecture, CQRS, Rich Domain Model, MediatR, and the Transactional Outbox pattern.

This project is designed as a real-world reference implementation beyond traditional CRUD-based applications.

---

## 🚀 Features

- Domain-Driven Design (DDD)
- Clean Architecture
- Rich Domain Model
- CQRS pattern
- MediatR integration
- Transactional Outbox Pattern
- Background Worker (OutboxProcessor)
- EF Core advanced mapping (Owned Entities & Value Objects)
- Domain Events
- Unit Of Work pattern

---

## 🧱 Architecture

The solution follows layered architecture:

```
src/
 ├── Domain
 │     ├── Aggregates
 │     ├── Entities
 │     ├── ValueObjects
 │     └── Domain Events
 │
 ├── Application
 │     ├── Commands (CQRS)
 │     ├── Queries
 │     └── Abstractions
 │
 ├── Infrastructure
 │     ├── EF Core Persistence
 │     ├── Repositories
 │     └── Outbox Worker
 │
 └── Api
       └── Minimal API endpoints
```

---

## 🧠 Key Concepts

### Rich Domain Model

Business logic is encapsulated inside domain entities instead of services.

### Transactional Outbox Pattern

Domain events are stored in the database during `SaveChanges()` and processed asynchronously by a background worker.

Benefits:

- Reliable event publishing
- No data loss
- Eventual consistency
- Scalable architecture

### CQRS

Commands and Queries are separated:

- Commands change state
- Queries read data

---

## ⚙️ Setup

### 1. Configure database

Edit:

```
Api/appsettings.json
```

```json
"ConnectionStrings": {
  "Db": "Server=.\\MSSQLSERVER2025;Database=AdvancedDDDDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

### 2. Apply migrations

Package Manager Console:

```
Add-Migration Init -Project Infrastructure -StartupProject Api
Update-Database -Project Infrastructure -StartupProject Api
```

---

### 3. Run API

```
dotnet run --project src/Api
```

---

## 🔥 API Endpoints

### Create Order

```
POST /orders
```

Body:

```json
{
  "Email": "test@example.com"
}
```

---

### Add Order Item

```
POST /orders/{id}/items
```

---

### Confirm Order

```
POST /orders/{id}/confirm
```

---

### Pay Order

```
POST /orders/{id}/pay?paymentRef=XXX
```

---

### Get Confirmed Orders

```
GET /orders/confirmed
```

---

## 📬 Outbox Processor

A background hosted service processes domain events stored in the Outbox table.

Ensures:

- Reliable processing
- Concurrency-safe updates
- Event-driven architecture readiness

---

## 🧩 Tech Stack

- .NET
- ASP.NET Core Minimal API
- EF Core
- MediatR
- SQL Server

---

## 🎯 Purpose

This repository is intended as:

- Advanced DDD learning reference
- Enterprise architecture example
- Clean architecture sample for real-world systems

---

## 👨‍💻 Author

Built as an advanced DDD sample demonstrating enterprise-grade architecture patterns.
