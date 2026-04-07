# 🎓 Student Course Management Platform

A backend system designed to manage the **full lifecycle of users and courses in an academic environment** — including secure authentication, role-based access control, and student enrollment.

This project simulates how real-world platforms handle **user identity, permissions, and course interactions**.

---

## 🎯 Purpose of the Project

This project focuses on building a **secure and structured backend system** that demonstrates:

* Authentication & authorization flows
* Role-based permissions (Admin, Teacher, Student)
* Student enrollment into courses
* Clean architecture and maintainable code

It emphasizes **security, scalability, and real-world backend patterns**.

---

## ✨ Core Features

* 🔐 JWT Authentication & Role-Based Authorization
* 👥 Multiple user roles (Admin, Teacher, Student)
* 📧 Email verification & password reset
* 🔁 Refresh tokens via HttpOnly cookies
* 📚 Course creation and management
* 🎓 Student enrollment system
* ⚠️ Global error handling & structured logging
* 📦 Standardized API responses
* 🧩 Clean layered architecture
* 🧪 Unit tested services (xUnit + Moq)

---

## 👤 User Roles & Capabilities

### Admin

* Full system control
* Manage users and courses
* Oversee platform data

### Teacher

* Create and manage courses
* Update course content
* Delete courses they manage

### Student

* Register and verify account
* Enroll in courses
* View enrolled courses
* Manage personal profile

---

## 🔐 Authentication Flow (Key Highlight)

The system implements a **complete authentication lifecycle**:

1. User registers → receives email verification code
2. Email must be verified before login
3. Login returns JWT access token + refresh token (HttpOnly cookie)
4. Access token expires → refreshed securely
5. Password reset via secure, time-limited token

This reflects **production-grade authentication systems**.

---

## 🎓 Enrollment System

Students can:

* Enroll in courses
* Retrieve their enrolled courses

```http
POST /api/enrollment/enroll/{courseId}
GET /api/enrollment/my-courses
```

> 🔒 Enrollment actions are restricted to **authenticated students only**.

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
* SMTP (Gmail)
* Custom API response factory

---

## 🧠 Architecture Overview

The project follows a clean structure:

* **Controllers** → Handle HTTP requests
* **Services** → Business logic (Auth, User, Course, Enrollment)
* **Data Access Layer** → EF Core database interaction
* **DTOs** → Safe data transfer
* **Core Utilities** → Shared responses & helpers

---

## ⚙️ Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/student-course-management.git
cd student-course-management
```

### 2. Configure database

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

---

## 🔐 Authentication Endpoints

| Method | Endpoint                    | Description       |
| ------ | --------------------------- | ----------------- |
| POST   | `/api/auth/register`        | Register new user |
| POST   | `/api/auth/verify-email`    | Verify email      |
| POST   | `/api/auth/login`           | Login user        |
| POST   | `/api/auth/refresh-token`   | Refresh token     |
| POST   | `/api/auth/forgot-password` | Request reset     |
| POST   | `/api/auth/reset-password`  | Reset password    |

---

## 👥 User Management (Admin Only)

| Method | Endpoint         |
| ------ | ---------------- |
| GET    | `/api/user`      |
| GET    | `/api/user/{id}` |
| POST   | `/api/user`      |
| PUT    | `/api/user/{id}` |
| DELETE | `/api/user/{id}` |

---

## 📚 Course Management

| Method | Endpoint           | Access          |
| ------ | ------------------ | --------------- |
| GET    | `/api/course`      | Public          |
| POST   | `/api/course`      | Admin / Teacher |
| PUT    | `/api/course/{id}` | Admin / Teacher |
| DELETE | `/api/course/{id}` | Admin / Teacher |

---

## 🧪 Testing

Unit tests focus on authentication and service logic:

* Registration & validation
* Email verification
* Login scenarios
* Password reset
* Token handling

Run tests:

```bash
dotnet test
```

---

## 🌐 Live Demo

🔗 https://your-deployment-url.up.railway.app/swagger/index.html

> ⚠️ Email verification is auto-confirmed in demo mode.

---

## 🚀 Future Improvements

* Course ownership validation (teacher-specific control)
* Student progress tracking
* Instructor dashboard
* File uploads (course materials)
* Real-time notifications (SignalR)
* Integration tests

---

## 📌 Project Status

* ✅ Fully functional backend API
* ✅ Secure authentication system
* ✅ Multi-role authorization
* ✅ Enrollment system implemented
* ✅ Production-style architecture

---

## 💡 Final Notes

This project demonstrates the ability to build a **secure, multi-role backend system** with real-world features such as authentication flows, role-based access control, and user-course relationships.

It reflects practical backend engineering skills including:

* API design
* Security best practices
* Clean architecture
* Scalable service structure
