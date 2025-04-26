# Pawesome Project Startup Guide

This guide will help you set up the Pawesome MVC project for development, using Docker only for the database and mail service.

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

## Project Architecture

The Pawesome project is structured according to MVC (Model-View-Controller) principles with a modular organization:

- **Extensions** - Extension classes to simplify configuration:
  - `ServiceCollectionExtensions.cs`: Configures services (database, identity, validations, etc.)
  - `ApplicationBuilderExtensions.cs`: Configures the HTTP pipeline
  - `WebApplicationExtensions.cs`: Manages database initialization

- **Layered Architecture**:
  - **Controllers**: Handle HTTP requests
  - **Models**: Define entities and DTOs
  - **Views**: User interface
  - **Services**: Business logic
  - **Repositories**: Data access
  - **Validators**: Data validation with FluentValidation

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