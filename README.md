# Movie Ticket Booking Web Application (Web Đặt Vé Xem Phim)

An ASP.NET Core MVC-based web application designed for online movie ticket booking. This platform allows users to browse movies, view showtimes, select seats, and book tickets, while offering an administrative panel for managing showtimes, movies, genres, users, and bookings.

## Features

- **User Authentication**: Secure local register/login with session state, as well as integrated registration and authentication via Google OAuth.
- **Movie Catalog**: Dynamic display of movies categorized by genres, featuring "Now Showing" and "Coming Soon" sections.
- **Seat Booking System**: Interactive, real-time seat status visualization allowing users to select seats for specific showtimes.
- **Admin Dashboard**: Comprehensive management interface for administrators to manage Movies, Genres, Showtimes, Seats, Tickets, Bookings, and User roles.
- **Notification Services**: Integrated SMTP configuration for email alerts (e.g., ticket booking confirmations).

---

## Tech Stack

- **Backend**: ASP.NET Core MVC (C#)
- **Database**: PostgreSQL (via Entity Framework Core and Npgsql)
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap
- **Authentication**: Cookie-based Authentication & Google OAuth

---

## Getting Started

### Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) (or matching version)
- [PostgreSQL](https://www.postgresql.org/) database server

### 1. Database Setup

A schema dump with sample data is provided in `movie.sql`. Create a database named `movie` on your PostgreSQL server and import this file:

```bash
psql -U postgres -d movie -f movie.sql
```

### 2. Configuration

Update the PostgreSQL connection string and configure your Google OAuth credentials in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ckContext": "Host=127.0.0.1;Database=movie;Username=your_username;Password=your_password;Persist Security Info=True;Include Error Detail=True;"
  },
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  }
}
```

> [!NOTE]
> The database connection can be further configured or troubleshot in `Program.cs`. Ensure that your local PostgreSQL instance is running and reachable at the host/port specified.

### 3. Build & Run

Restore dependencies and run the application using the dotnet CLI:

```bash
dotnet restore
dotnet run
```

The application will launch and listen on `https://localhost:7112` or `http://localhost:5115` by default (check `Properties/launchSettings.json` for details).

---

## Project Structure

```text
├── Controllers/         # MVC Controllers (Authentication, Booking, Admin views, etc.)
├── Models/              # Database models, viewmodels, and data structures
├── Views/               # Razor View components and layout templates
├── Data/                # Entity Framework Core db context and initialization
├── wwwroot/             # Static assets (images, css, javascript, webfonts)
├── Program.cs           # Application bootstrap, DI container, and middleware pipeline
└── movie.sql            # Database schema and initial seed data
```
