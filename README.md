# 🎓 Student Course Management Platform

A backend system designed to handle the **full lifecycle of student accounts** in an academic environment — from secure registration and email verification to authentication and course access.

This project focuses on **identity management, security, and user workflows**, simulating how modern platforms manage users and protected resources.

---

## 🎯 Purpose of the Project

This project was built to demonstrate how a real-world system handles:

* Secure user authentication and authorization
* Email verification and password recovery flows
* Token-based session management
* Role-based access control

It emphasizes **security, reliability, and clean architecture**, rather than just CRUD operations.

---

## ✨ Core Features

* 🔐 JWT Authentication & Role-Based Authorization
* 📧 Email verification with time-limited codes
* 🔁 Refresh tokens stored in HttpOnly cookies
* 🔑 Forgot & reset password flow
* 👥 Role system (Admin / Student)
* 📚 Course management (Admin-controlled)
* ⚠️ Global error handling & structured logging
* 📦 Standardized API responses
* 🧩 Clean layered architecture
* 🧪 Unit tested services (xUnit + Moq)

---

## 👤 User Roles & Capabilities

### Admin

* Full control over users and courses
* Create, update, and delete courses
* Manage platform data

### Student

* Register and verify account via email
* Log in securely
* Access available courses
* Manage personal profile

---

## 🔐 Authentication Flow (Key Highlight)

This project implements a **complete authentication system**:

1. User registers → receives verification code via email
2. Email must be verified before login
3. Login returns JWT access token + refresh token (HttpOnly cookie)
4. Access token expires → refreshed securely via cookie
5. Password reset flow uses secure, time-limited tokens

This mirrors how **production-grade systems handle identity and security**.

---

## 🛠️ Tech Stack

* **Backend:** ASP.NET Core Web API
* **Database:** PostgreSQL (production) / SQL Server (local) + EF Core
* **Authentication:** JWT + Refresh Tokens (HttpOnly cookies)
* **Architecture:** Layered (Controllers → Services → Data Access)

---

## 📚 Libraries & Tools

* AutoMapper
* FluentValidation
* Serilog (structured logging)
* xUnit + Moq (unit testing)
* Rate Limiting
* User Secrets
* SMTP (Gmail) for email services
* Custom API response factory

---

## 🧠 Architecture Overview

The project follows a clean and maintainable structure:

* **Controllers** → Handle HTTP requests
* **Services** → Contain business logic (Auth, User, Course)
* **Data Access Layer** → Database interaction via EF Core
* **DTOs** → Safe data transfer between layers
* **Core Utilities** → Shared responses, enums, helpers

---

## ⚙️ Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/nucanasaraia/StudentCourseManagement.git
cd StudentCourseManagement
```

### 2. Configure database

Update your `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "your_connection_string_here"
}
```

### 3. Set user secrets

```bash
dotnet user-secrets set "SMTP:SenderEmail" "your_email@gmail.com"
dotnet user-secrets set "SMTP:AppPassword" "your_app_password"
dotnet user-secrets set "Jwt:Key" "your_super_secret_key"
```

### 4. Apply migrations

```bash
dotnet ef database update
```

### 5. Run the project

```bash
dotnet run
```

---

## 📬 API Documentation

After running the project, open:

```
https://localhost:{port}/swagger
```

Use Swagger UI to explore and test endpoints.

---

## 🔐 Key Endpoints (Auth Focus)

| Method | Endpoint                    | Description              |
| ------ | --------------------------- | ------------------------ |
| POST   | `/api/auth/register`        | Register new student     |
| POST   | `/api/auth/verify-email`    | Verify email with code   |
| POST   | `/api/auth/login`           | Login and receive tokens |
| POST   | `/api/auth/refresh-token`   | Refresh access token     |
| POST   | `/api/auth/forgot-password` | Request reset link       |
| POST   | `/api/auth/reset-password`  | Reset password           |

---

## 📚 Course Management

| Method | Endpoint            | Access  |
| ------ | ------------------- | ------- |
| GET    | `/api/courses`      | Student |
| POST   | `/api/courses`      | Admin   |
| PUT    | `/api/courses/{id}` | Admin   |
| DELETE | `/api/courses/{id}` | Admin   |

---

## 🧪 Testing

Unit tests are written using **xUnit** and **Moq**, focusing on core authentication logic.

### Covered scenarios:

* Registration (valid / duplicate email)
* Email verification (valid / expired / incorrect code)
* Login (valid / invalid / unverified user)
* Password reset flow
* Token handling

Run tests:

```bash
dotnet test
```

---

## 🌐 Live Demo

🔗 https://your-deployment-url.up.railway.app/swagger/index.html

> ⚠️ Email verification is auto-confirmed in the live demo. Full functionality works locally with SMTP configuration.

---

## 🚀 Future Improvements

* Student progress tracking
* Role expansion (Instructor role)
* File uploads (course materials)
* Real-time notifications (SignalR)
* Integration testing

---

## 📌 Project Status

* ✅ Fully functional backend API
* ✅ Secure authentication system
* ✅ Email verification & password recovery
* ✅ Unit tested core services
* ✅ Production-style architecture

---

## 💡 Final Notes

This project is focused on **building a secure and realistic authentication system**, reflecting how modern applications manage users, sessions, and sensitive operations.

It demonstrates a strong understanding of:

* Authentication flows
* API design
* Security best practices
* Maintainable backend architecture
