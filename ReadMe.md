# 🚀 Full-Stack Todo Management App

A modern, secure, and responsive Todo management system built with **.NET 9**, **Blazor WebAssembly**, and **Entity Framework Core**. This project demonstrates a complete implementation of a decoupled architecture, focusing on security, database relationships, and professional UI/UX.

## ✨ Key Features

* **Secure Authentication**: Full login and registration system using **JWT (JSON Web Tokens)** stored in LocalStorage.
* **Relational Database**: Managed with **Entity Framework Core**, featuring a 1:N relationship between Users, Categories, and Todo items.
* **Dynamic Categorization**: Create, delete, and organize tasks into custom categories.
* **Real-time UI**: Responsive Blazor WebAssembly frontend with **Bootstrap 5** and **Bootstrap Icons**.
* **Advanced Grouping**: Automatic grouping of tasks by category using **LINQ GroupBy** logic for better organization.
* **Clean Architecture**: Separation of concerns with a **Shared** project for DTOs (Data Transfer Objects).

## 🛠️ Tech Stack

* **Frontend**: Blazor WebAssembly, Bootstrap 5.
* **Backend**: .NET 9 Web API (Minimal APIs).
* **Database**: SQL Server / LocalDB.
* **ORM**: Entity Framework Core.
* **Security**: JWT Bearer Authentication.

## 🏗️ Project Structure

* `Client/`: The Blazor WebAssembly frontend application.
* `Server/`: The Web API backend, handling business logic and database access.
* `Shared/`: Common models and DTOs used by both Client and Server.

---

## 🚀 Getting Started

### Prerequisites
* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* Visual Studio 2022 or VS Code

### Setup
1.  **Clone the repository**:
    ```bash
    git clone [https://github.com/your-username/your-repo-name.git](https://github.com/your-username/your-repo-name.git)
    ```

2.  **Update the Database**:
    Navigate to the `Server` project folder and run:
    ```bash
    dotnet ef database update
    ```

3.  **Run the application (Visual Studio Multiple Startup)**:
    To run the application, you need to start both the Server and the Client projects simultaneously:
    
    * Right-click on the **Solution** in the Solution Explorer.
    * Select **Properties**.
    * Go to **Common Properties** > **Startup Project**.
    * Select **Multiple startup projects**.
    * Set the Action to **Start** for both the `Server` and `Client` projects.
    * Click **Apply** and press **F5** to start debugging.

---

## 🔒 Security & Configuration Note



For portfolio and local development purposes, this project includes a default `appsettings.json` with a development JWT key and local database connection strings to ensure the application is "ready-to-run". 

However, in a **production environment**, I am aware of the following best practices:
* **Sensitive Data**: The `Jwt:Key` and Connection Strings should be moved to **Environment Variables**, **User Secrets**, or **Azure Key Vault**.
* **Hardcoded Secrets**: No sensitive keys should be tracked in the Git history.
* **Token Validation**: The current implementation validates Issuer and Audience via centralized configuration for consistency.

---

### 💡 What I Learned During This Project
* Implementing **JWT-based authentication** and managing authorization headers in Blazor.
* Using **Eager Loading (`.Include()`)** to optimize database queries and prevent data gaps.
* Handling complex **One-to-Many relationships** in EF Core.
* Debugged full-stack communication issues and managed **CORS/API routing**.
* Organizing UI layouts with **Bootstrap Accordions** and dynamic grouping logic.
