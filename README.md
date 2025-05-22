# 🚗 LaWash Smart Parking Backend (IoT)

Backend API for managing a smart parking lot system that integrates with IoT devices (e.g., Raspberry PI) to track vehicle occupancy in real time.

Built using **ASP.NET Core 8**, following **Clean Architecture**, **SOLID principles**, and **Domain-Driven Design (DDD)** patterns.

---

## 📦 Project Structure

LaWash.IoT <br>
├── API → Controllers and HTTP configuration <br>
├── Application → Use cases, services, DTOs <br>
├── Domain → intermediate logic like UnitOfWork and validations <br>
├── Infrastructure → Persistence <br>
├── Transversal → Shared exceptions, constants, helpers, middlewares <br>
└── Tests <br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── API.Tests → Controller-level tests (mocked IParkingApplication) <br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;└── Application.Tests → Business logic tests (mocked IUnitOfWork, IRateLimiter) <br>


---

## ⚙️ Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- (Optional) [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [Rider](https://www.jetbrains.com/rider/)
- (Optional) [Postman](https://www.postman.com/) or [Insomnia](https://insomnia.rest/) for testing the API

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/DevJaimeGR/LaWash.IoT.Interview.git
cd LaWash.IoT.Interview
```

### 2. Restore dependencies
```bash
dotnet restore
```

### 3. Build the solution
```bash
dotnet build
```

### 4. Run the API
```bash
cd LaWash.Iot.API
dotnet run
```

---

## 📫 API Endpoints

Just to clarify, the `/api/devices` endpoint was added solely to visualize which devices are related to a default spot and which are not, in order to test the different test cases.

| Method | Endpoint                         | Description                     |
| ------ | -------------------------------- | ------------------------------- |
| GET    | `/api/devices`                   | List all registered devices     |
| POST   | `/api/parking-spots/{id}/occupy` | Mark a spot as occupied         |
| POST   | `/api/parking-spots/{id}/free`   | Mark a spot as free             |
| GET    | `/api/parking-spots`             | Get all parking spot statuses   |
| POST   | `/api/parking-spots`             | Create a new parking spot       |
| DELETE | `/api/parking-spots/{id}`        | Delete an existing parking spot |

---

##🧪 Running Tests

### 3. Build the solution
```bash
dotnet test
```
Tests are divided between:

LaWash.IoT.API.Tests: tests for ParkingController

LaWash.IoT.Application.Tests: tests for business logic (e.g. rate limiting, status changes)

---

##🧠 Features
✅ Clean Architecture (Controllers → Application →  Domain → Repository)

✅ In-memory Repository Pattern (simulated persistence)

✅ Rate Limiting (1 request every 10 seconds per device)

✅ Custom exception handling (BadResponseWithMessage)

✅ Pagination for all GET endpoints

✅ Testable, mockable, and scalable code

---

##🛠 Technologies

°ASP.NET Core 8

°C# 12

°xUnit & Moq

°FluentAssertions

°Swagger (OpenAPI)

°RESTful API

---
##🧑‍💻 Author

Jaime Ivan Gonzalez Rincon <br>
Senior Backend Developer <br>
[LinkenId](https://www.linkedin.com/in/jaime-ivan-gonzalez-rincon)

