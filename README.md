# Pawesome — Pet Sitting Web Application

## Project Context

**Pawesome** is a web application built with **.NET MVC (C#)** that enables users to:

* Offer their services as **pet sitters**
* Request a **pet sitter** to care for their animals

Inspired by platforms such as Airbnb or Vinted, the aim is to deliver a secure, smooth, and intuitive experience tailored to pet sitting.

---

## Main Features

* Secure authentication using ASP.NET Identity and Google OAuth2
* Real-time messaging system with SignalR
* Address and location management (countries, cities)
* Creation of pet profiles
* Posting and browsing of sitting advertisements (offers and requests)
* Booking functionality with integrated Stripe payments (including escrow handling)
* Booking tracking (confirmation, rating, and dispute management)
* Real-time notifications
* Automated emails (confirmations, reminders, alerts)

---

## Technical Architecture

The application is structured around the **MVC + Services + Repositories** design pattern to ensure a clear separation of concerns.

### Structure Overview

* **MVC**: Controllers, Razor Views (`.cshtml`), ViewModels
* **Services**: Encapsulated business logic
* **Repositories**: Data access layer
* **Infrastructure**: Utility classes (helpers, extensions, mappers, validations)
* **Real-time**: SignalR hubs
* **Validation**: FluentValidation (server-side) and jQuery (client-side)
* **Database**: PostgreSQL via Entity Framework Core (Code First)
* **Mapping**: AutoMapper

### Frontend

* Server-rendered views using **Razor Pages**
* Modular CSS without external frameworks (e.g., Bootstrap)
* Structured use of **native CSS** and **jQuery** for interactivity and form validation

CSS organization:

```
wwwroot/
├── css/
│   ├── base/
│   ├── components/
│   ├── layouts/
│   ├── pages/
├── js/
├── lib/
├── jquery/
```

---

## Repositories & Services

### Repositories

* `UserRepository`
* `PetRepository`
* `AnimalTypeRepository`
* `AdvertRepository`
* `MessageRepository`
* `PaymentRepository`
* `CityRepository`
* `CountryRepository`
* `AddressRepository`
* `NotificationRepository`
* `BookingRepository`

### Services

* `AuthService`
* `UserService`
* `PetService`
* `EmailService`
* `AdvertService`
* `AnimalTypeService`
* `MessageService`
* `PaymentService`
* `NotificationService`
* `LocationService`
* `BookingService`

---

## Key Technologies

| Category       | Technologies                        |
| -------------- | ----------------------------------- |
| Backend        | ASP.NET Core MVC, C#                |
| Frontend       | Razor Pages (.cshtml), CSS, jQuery  |
| Real-time      | SignalR                             |
| ORM            | Entity Framework Core, PostgreSQL   |
| Validation     | FluentValidation, jQuery Validation |
| Payments       | Stripe                              |
| Object Mapping | AutoMapper                          |
| Authentication | ASP.NET Identity, Google OAuth2     |
| Architecture   | MVC, Services, Repositories         |

---

## Project Structure (Overview)

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

## Notes

* The application avoids heavy frontend frameworks such as React or Vue.
* Emphasis is placed on maintainability, modularity, and secure application design.

---

# Getting Started

This guide explains how to set up the Pawesome project for development using Docker (for the database and mail services).

## Prerequisites

* [Docker Desktop](https://www.docker.com/products/docker-desktop)
* [.NET 9 SDK](https://dotnet.microsoft.com/download)
* [Git](https://git-scm.com)

## Installation

1. Clone the repository:

   ```bash
   git clone https://CCICampus@dev.azure.com/CCICampus/CDA-ALT2425-G3/_git/CDA-ALT2425-G3
   cd Pawesome
   ```

2. Ensure Docker is running

3. Confirm the presence of `compose.dev.yaml` at the project root

4. Restore dependencies:

   ```bash
   dotnet restore
   ```

## Running the Application

1. Start the Docker containers:

   ```bash
   docker compose -f compose.dev.yaml up -d
   ```

2. Launch the application:

   ```bash
   dotnet run
   ```

Visit [http://localhost:5159/](http://localhost:5159/) to access the application.

---

## Docker Services

### PostgreSQL

* Host: `localhost`
* Port: `5434`
* Username: `pawesome`
* Password: `pawesome`
* Database: `pawesome`

### MailHog (Email Testing)

Used to intercept outgoing emails during development.

* Web UI: [http://localhost:8025/](http://localhost:8025/)
* SMTP: `localhost:1025`

---

## Stopping the Application

To stop the application:

* Use `Ctrl+C` in the terminal running the application

To stop and remove containers:

```bash
docker compose -f compose.dev.yaml down
```

To remove volumes as well:

```bash
docker compose -f compose.dev.yaml down -v
```
