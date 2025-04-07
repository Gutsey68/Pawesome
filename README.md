# Pawesome Project - Getting Started Guide

This guide will help you set up the Pawesome MVC project for development using Docker only for the database.

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop/) installed and running
- [.NET 9 SDK](https://dotnet.microsoft.com/download) installed locally
- [Git](https://git-scm.com/downloads) to clone the repository
- Basic knowledge of .NET Core and Docker

## Installation Instructions

1. Clone the repository:
   ```bash
   git clone https://CCICampus@dev.azure.com/CCICampus/CDA-ALT2425-G3/_git/CDA-ALT2425-G3
   cd Pawesome
   ```

2. Verify that Docker is running on your machine

3. Make sure the `compose.dev.yaml` file is present in the project root directory

## Starting the Project

1. First launch the database container:

```bash
docker compose -f compose.dev.yaml up -d
```

2. Navigate to the project directory and run the application:

```bash
cd Pawesome
dotnet run
```

The application will be available at http://localhost:5159/

## Database Connection

The PostgreSQL database runs on:
- Host: localhost
- Port: 5434 (mapped from 5432 inside the container)
- Username: pawesome
- Password: pawesome
- Database name: pawesome

## Stopping the Application

To stop the application, press `Ctrl+C` in the terminal where it's running.

To stop the database container:

```bash
docker compose -f compose.dev.yaml down
```

To also remove volumes (database data):

```bash
docker compose -f compose.dev.yaml down -v
```