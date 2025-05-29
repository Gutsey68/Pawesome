# 🐾 Pawesome — Pet Sitting Application

## 📌 Context

**Pawesome** is a web application developed in **.NET MVC (C#)** that allows users to:

- **offer** their services as **pet sitters**,
- **request** a pet sitter to take care of their animals.

The goal is to provide a secure, smooth, and intuitive platform, similar to services like Airbnb or Vinted, but for pet
sitting.

---

## 🎯 Main Features

- 🔐 Secure authentication (via Identity & OAuth Google)
- 📬 **Real-time messaging** system (SignalR)
- 📍 **Address** management (countries, cities) and location
- 🐕 Creation of **pets** with individual profiles
- 📢 Creation and browsing of sitting **advertisements** (offers / requests)
- 📆 **Booking** advertisements with **secure Stripe payment** (with escrow)
- 🧾 Booking tracking: post-service validation, rating, disputes
- 🔔 Real-time **notifications**
- 📧 Automated email sending (confirmation, reminder, alert...)

---

## 🏗️ Technical Architecture

The application follows a clean separation based on the **MVC + Services + Repositories** architecture, with a clear
abstraction layer between business logic, data access, and presentation.

### Technical Structure

- **MVC**: Controllers, Razor Views (`.cshtml`), ViewModels
- **Services**: Isolated business logic
- **Repositories**: Data access
- **Infrastructure**: Helpers, Extensions, Mappers, Validations
- **Real-time communication**: SignalR Hubs
- **Validation**: FluentValidation (backend) + jQuery (frontend)
- **Database**: PostgreSQL (via Entity Framework Code First)
- **Object mapping**: AutoMapper

### Frontend

- Pages in **Razor (.cshtml)** with **modular CSS**
- CSS structure:

```
wwwroot/
css/
base/
components/
layouts/
pages/
js/
lib/
jquery/
```

- No CSS framework (like Bootstrap), only structured **native CSS** + **jQuery** for client validation

---

## 🔌 Services & Repositories

### 🔄 Repositories

- `UserRepository`
- `PetRepository`
- `AnimalTypeRepository`
- `AdvertRepository`
- `MessageRepository`
- `PaymentRepository`
- `CityRepository`
- `CountryRepository`
- `AddressRepository`
- `NotificationRepository`
- `BookingRepository`

### ⚙️ Services

- `AuthService`
- `UserService`
- `PetService`
- `EmailService`
- `AdvertService`
- `AnimalTypeService`
- `MessageService`
- `PaymentService`
- `NotificationService`
- `LocationService`
- `BookingService`

---

## ⚙️ Key Technologies

| Category       | Tools / Technologies                |
|----------------|-------------------------------------|
| Backend        | ASP.NET Core MVC, C#                |
| Frontend       | Razor Pages (.cshtml), CSS, jQuery  |
| Real-time      | SignalR                             |
| ORM            | Entity Framework Core + PostgreSQL  |
| Validation     | FluentValidation, jQuery Validation |
| Payment        | Stripe                              |
| Object mapping | AutoMapper                          |
| Authentication | ASP.NET Identity + OAuth (Google)   |
| Architecture   | MVC, Services, Repositories         |

---

## 🗂️ Project Structure (excerpts)

```plaintext
Pawesome/
├── Controllers/
├── Data/
├── Docs/
├── Helpers/
├── Hubs/
├── Infrastructure/
│   ├── Extensions/
│   ├── Mappers/
├── Migrations/
├── Models/
│   ├── Configuration/
│   ├── Dtos/
│   ├── Entities/
│   ├── Enums/
│   ├── ViewModels/
├── Repositories/
├── Services/
├── Validators/
├── Views/
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── lib/
│   ├── images/
```

---

## 📝 Notes

* The project contains **no heavy frontend dependencies** (like React/Vue).
* The emphasis is on **architectural clarity**, **security**, and **maintainability**.

---

# Project Startup Guide

This guide will help you set up the Pawesome MVC project for development, using Docker only for the database and mail
service.

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop/) installed and running
- [.NET 9 SDK](https://dotnet.microsoft.com/download) locally installed
- [Git](https://git-scm.com/downloads) to clone the repository

## Installation Instructions

1. Clone the repository:
   ```bash
   git clone https://CCICampus@dev.azure.com/CCICampus/CDA-ALT2425-G3/_git/CDA-ALT2425-G3
   cd Pawesome
   ```

2. Verify that Docker is running on your machine

3. Make sure the `compose.dev.yaml` file is in the project root directory

4. Restore the project dependencies:
   ```bash
   dotnet restore
   ```

## Starting the Project

1. First, launch the Docker containers:

```bash
docker compose -f compose.dev.yaml up -d
```

2. Navigate to the project directory and run the application:

```bash
cd Pawesome
dotnet run
```

The application will be available at http://localhost:5159/

## Docker Services

### PostgreSQL Database

- Host: localhost
- Port: 5434 (mapped from 5432 in the container)
- Username: pawesome
- Password: pawesome
- Database name: pawesome

### MailHog (Email Testing Service)

A MailHog service is included to intercept and visualize emails sent by the application during development.

- Web interface: http://localhost:8025/
- SMTP Server: localhost:1025

All emails sent by the application will be captured by MailHog and visible in the web interface.

## Stopping the Application

To stop the application, press `Ctrl+C` in the terminal where it's running.

To stop the Docker containers:

```bash
docker compose -f compose.dev.yaml down
```

To also remove volumes (database data):

```bash
docker compose -f compose.dev.yaml down -v
```