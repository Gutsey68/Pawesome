# 🐾 Pawesome — Pet Sitting Application

## 📌 Context

**Pawesome** is a web application developed in **.NET MVC (C#)** that allows users to:
- **offer** their services as **pet sitters**,
- **request** a pet sitter to take care of their animals.

The goal is to provide a secure, smooth, and intuitive platform, similar to services like Airbnb or Vinted, but for pet sitting.

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

The application follows a clean separation based on the **MVC + Services + Repositories** architecture, with a clear abstraction layer between business logic, data access, and presentation.

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

| Category             | Tools / Technologies                          |
|----------------------|-----------------------------------------------|
| Backend              | ASP.NET Core MVC, C#                          |
| Frontend             | Razor Pages (.cshtml), CSS, jQuery            |
| Real-time            | SignalR                                       |
| ORM                  | Entity Framework Core + PostgreSQL            |
| Validation           | FluentValidation, jQuery Validation           |
| Payment              | Stripe                                        |
| Object mapping       | AutoMapper                                    |
| Authentication       | ASP.NET Identity + OAuth (Google)             |
| Architecture         | MVC, Services, Repositories                   |

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
│   ├── Filters/
│   ├── Mappers/
│   ├── Validators/
├── Migrations/
├── Models/
│   ├── Configuration/
│   ├── Dtos/
│   ├── Entities/
│   ├── ViewModels/
├── Repositories/
├── Services/
├── Views/
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── lib/
│   ├── images/
│   ├── svg/
```

---

## 📝 Notes

* The project contains **no heavy frontend dependencies** (like React/Vue).
* The emphasis is on **architectural clarity**, **security**, and **maintainability**.
* The **deferred payment with final validation** system is inspired by platforms like Airbnb and Vinted.

---

// 👇 OBJECTIVE: Implement a booking workflow for a pet sitting platform
// Inspired by Airbnb/Vinted: payment is authorized at booking (Stripe) but captured only after service validation.
// Booking can be a "sitting request" or a "sitting offer".
//
// Expected features:
// 1. Create a booking with "pending_confirmation" status
// 2. Stripe payment with `capture_method = manual` for authorization only
// 3. Notification to pet sitter to accept/reject
// 4. If accepted => "accepted" status, otherwise "declined"
// 5. After the sitting, the user must validate the service
// 6. Payment captured on validation (or automatically after X days)
// 7. Statuses to manage: pending_confirmation, accepted, declined, in_progress, completed, disputed, cancelled, expired
//
// Also requires: booking records, Stripe webhooks for payments, notifications via SignalR
// Uses these services: IPaymentService, IAdvertService, INotificationService, IMessageService
// Database: PostgreSQL with EF Core
//
// Help me define the business steps (services, statuses, validations) and generate the necessary classes/models for this flow.
