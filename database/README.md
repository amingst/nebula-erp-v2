# Nebula ERP Database Setup

This directory contains the PostgreSQL database configuration for the Nebula ERP system.

## Quick Start

1. **Start the database:**

    ```bash
    docker-compose up -d postgres
    ```

2. **Start with PgAdmin (optional):**

    ```bash
    docker-compose up -d
    ```

3. **Stop the database:**
    ```bash
    docker-compose down
    ```

## Database Access

### Admin Access

-   **Host:** localhost:5432
-   **Database:** nebula_erp
-   **Username:** nebula_admin
-   **Password:** nebula_dev_password

### PgAdmin Web Interface

-   **URL:** http://localhost:8080
-   **Email:** admin@nebula.local
-   **Password:** admin123

## Architecture

### Schema Organization

```
nebula_erp/
├── auth schema          - User accounts, roles, JWT tokens
├── organizations schema - Organizations, memberships
├── accounting schema    - Financial transactions, accounts
├── hr schema           - Employee records, payroll
├── inventory schema    - Products, stock, movements
└── shared schema       - Cross-service shared data
```

### Service Users

Each service has its own database user with schema-specific permissions:

-   `auth_service` - Access to auth schema
-   `org_service` - Access to organizations schema + read auth for roles
-   `accounting_service` - Access to accounting schema
-   `hr_service` - Access to hr schema
-   `inventory_service` - Access to inventory schema

## Connection Strings

See `connection-strings.env` for ready-to-use connection strings for each service.

For .NET appsettings.json:

```json
{
	"ConnectionStrings": {
		"AuthDatabase": "Host=localhost;Port=5432;Database=nebula_erp;Username=auth_service;Password=auth_service_password;Search Path=auth;Include Error Detail=true"
	}
}
```

## Development Commands

### Backup Database

```bash
docker exec nebula-postgres pg_dump -U nebula_admin nebula_erp > backup.sql
```

### Restore Database

```bash
cat backup.sql | docker exec -i nebula-postgres psql -U nebula_admin -d nebula_erp
```

### Connect via psql

```bash
docker exec -it nebula-postgres psql -U nebula_admin -d nebula_erp
```

### View Logs

```bash
docker-compose logs postgres
```

## Production Notes

-   Change all default passwords before production deployment
-   Consider using Docker secrets for sensitive data
-   Set up proper backup and monitoring
-   Consider connection pooling (PgBouncer) for high-load scenarios
-   Use SSL connections in production environments

## Troubleshooting

### Database won't start

```bash
# Check logs
docker-compose logs postgres

# Remove data volume and restart (WARNING: deletes all data)
docker-compose down -v
docker-compose up -d postgres
```

### Connection refused

-   Ensure PostgreSQL container is running: `docker ps`
-   Check if port 5432 is available: `netstat -an | grep 5432`
-   Verify connection string parameters

### Performance optimization

-   The container includes `pg_stat_statements` extension for query analysis
-   Access via PgAdmin or psql to view query performance statistics
