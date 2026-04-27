# 🏖️ Resort Hub

**Resort Hub** is a full-featured villa booking web application built with ASP.NET Core MVC. It allows guests to browse and book villas, while admins manage listings, users, and reservations through a dedicated dashboard.

🌐 **Live Demo:** [https://resorthub.runasp.net/](https://resorthub.runasp.net/)

---

## ✨ Features

### 👤 Authentication & Authorization
- Register & Login with email/password
- OTP-based email confirmation (6-digit code, 15-minute expiry)
- Forgot password with OTP verification flow
- Google OAuth2 social login
- Role-based access control: **Admin** and **Customer**
- Cookie-based authentication with configurable session timeout

### 🏡 Villa Management (Admin)
- Create, edit, and delete villas with full details (name, description, price/night, capacity, sqft, location)
- Multi-image upload per villa via **Cloudinary**
- Set a main/cover image and manage display order
- Assign amenities to villas using a many-to-many relationship
- Toggle villa availability

### 📅 Booking Flow (Customer)
- Browse available villas from the home page or search
- View villa details with image gallery, amenities, and pricing
- Interactive date picker with real-time unavailable dates
- Multi-step booking: Preview → Payment → Confirmation
- Overlap/conflict detection before confirming a booking
- Draft booking stored in session during checkout
- View and cancel personal bookings

### 🛠️ Admin Dashboard
- Overview stats: total bookings, revenue, users
- Chart data with configurable time range (last N days)
- User management: search, filter by role, sort, paginate
- Toggle user active/inactive status
- Assign/change user roles
- Booking management: view, filter, update status, cancel

### 📧 Email Notifications
- HTML email templates for OTP confirmation and password reset
- Emails dispatched asynchronously via **Hangfire** background jobs
- SMTP delivery using **MailKit**

### 🖼️ Image Hosting
- All villa images hosted on **Cloudinary**
- Automatic cleanup on villa or image deletion

---

## 🗂️ Project Structure

```
Resort_Hub/
├── Controllers/         # MVC Controllers (Auth, Villa, Book, Admin, Account, Amenity)
├── Entities/            # EF Core domain models (Villa, Booking, ApplicationUser, Amenity...)
├── Services/            # Business logic layer (AuthService, VillaService, BookingService...)
├── ViewModels/          # View-specific models (Auth, Villa, Booking, Admin, Account)
├── Views/               # Razor views + shared layouts
├── Persistence/         # DbContext & repositories (Unit of Work pattern)
├── Configuration/       # Mapster mapping profiles & app configs
├── Settings/            # Strongly-typed settings (MailSettings, Cloudinary, FontAwesome)
└── DependencyInjection.cs  # Centralized service registration
```

---

## 🧰 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| Database | SQL Server + Entity Framework Core |
| Identity | ASP.NET Core Identity |
| Object Mapping | Mapster |
| Image Hosting | Cloudinary |
| Background Jobs | Hangfire |
| Email | MailKit (SMTP) |
| Auth (OAuth) | Google OAuth2 |
| Logging | Serilog |
| Error Handling | Global Exception Handler + ProblemDetails |

---

## ⚙️ Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server
- Cloudinary account
- Google OAuth credentials (optional)
- SMTP email credentials

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/resort-hub.git
   cd resort-hub
   ```

2. **Configure `appsettings.json`**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=...;Database=ResortHub;...",
       "HangfireConnection": "Server=...;Database=ResortHubHangfire;..."
     },
     "Cloudinary": {
       "CloudName": "your-cloud-name",
       "ApiKey": "your-api-key",
       "ApiSecret": "your-api-secret"
     },
     "MailSettings": {
       "Mail": "your@email.com",
       "DisplayName": "Resort Hub",
       "Password": "your-password",
       "Host": "smtp.gmail.com",
       "Port": 587
     },
     "Authentication": {
       "Google": {
         "ClientId": "your-client-id",
         "ClientSecret": "your-client-secret"
       }
     },
     "HangfireSettings": {
       "Username": "admin",
       "password": "admin123"
     }
   }
   ```

3. **Apply migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the app**
   - Main site: `https://localhost:5001`
   - Hangfire dashboard: `https://localhost:5001/jobs`

---

## 🗄️ Data Model

```
ApplicationUser (IdentityUser)
  └── Bookings[]

Villa
  ├── VillaImages[]
  ├── VillaAmenity[] ──── Amenity
  └── Bookings[]

Booking
  ├── Villa
  └── ApplicationUser

OtpEntry  (email confirmation & password reset)
```

---

## 👥 Team

| Name |
|---|
| Ibrahim Khaled |
| Abdallah Elsaid |
| Karim Hany |
| Mariam Fouda |
| Rawda Ashour |

---

## 📄 License

This project was built as an academic project at **ITI (Information Technology Institute)**.
