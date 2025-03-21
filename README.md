# Pawesome Project - Getting Started

This guide will help you set up the Pawesome MVC project for development using Docker.

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop/) installed and running
- [Git](https://git-scm.com/downloads) for cloning the repository
- Basic knowledge of .NET Core and Docker

## Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://CCICampus@dev.azure.com/CCICampus/CDA-ALT2425-G3/_git/CDA-ALT2425-G3
   cd pawesome-project
   ```

2. Check that Docker is running on your machine

3. Make sure the `compose.dev.yaml` file is present in the project root directory

## Running the Project

Start the application and database containers:

```bash
docker compose -f compose.dev.yaml up --build
```

This command:
- Builds the Docker image for the Pawesome application
- Starts the PostgreSQL database
- Sets up the development environment with hot reload

The application will be available at http://localhost:3000

## Development Workflow

- The project uses Entity Framework Core with PostgreSQL
- Source code is mounted as a volume, so changes to files in the `Pawesome` directory will be detected automatically
- The application is configured for hot reload with `DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true`

## Database Connection

The PostgreSQL database runs at:
- Host: localhost
- Port: 5434 (mapped from 5432 inside container)
- Username: pawesome
- Password: pawesome
- Database name: pawesome

## Stopping the Application

To stop the running containers, press `Ctrl+C` in the terminal or run:

```bash
docker compose -f compose.dev.yaml down
```

To remove volumes (database data) as well:

```bash
docker compose -f compose.dev.yaml down -v
```