-- Initialize Nebula ERP Database
-- This script sets up schemas for authentication and organizations services

\echo 'Creating schemas for Nebula ERP core services...'

-- Create schemas for core services
CREATE SCHEMA IF NOT EXISTS auth;
CREATE SCHEMA IF NOT EXISTS organizations; 

-- Grant permissions to nebula_admin
GRANT ALL PRIVILEGES ON SCHEMA auth TO nebula_admin;
GRANT ALL PRIVILEGES ON SCHEMA organizations TO nebula_admin;

-- Create service-specific users

-- Authentication service user
CREATE USER auth_service WITH PASSWORD 'auth_service_password';
GRANT CONNECT ON DATABASE nebula_erp TO auth_service;
GRANT USAGE ON SCHEMA auth TO auth_service;
GRANT CREATE ON SCHEMA auth TO auth_service;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA auth TO auth_service;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA auth TO auth_service;
ALTER DEFAULT PRIVILEGES IN SCHEMA auth GRANT ALL ON TABLES TO auth_service;
ALTER DEFAULT PRIVILEGES IN SCHEMA auth GRANT ALL ON SEQUENCES TO auth_service;

-- Organizations service user
CREATE USER org_service WITH PASSWORD 'org_service_password';
GRANT CONNECT ON DATABASE nebula_erp TO org_service;
GRANT USAGE ON SCHEMA organizations TO org_service;
GRANT CREATE ON SCHEMA organizations TO org_service;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA organizations TO org_service;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA organizations TO org_service;
ALTER DEFAULT PRIVILEGES IN SCHEMA organizations GRANT ALL ON TABLES TO org_service;
ALTER DEFAULT PRIVILEGES IN SCHEMA organizations GRANT ALL ON SEQUENCES TO org_service;

-- Cross-schema permissions for organization roles (temporary until SoC refactor)
GRANT USAGE ON SCHEMA auth TO org_service;
GRANT SELECT, UPDATE ON ALL TABLES IN SCHEMA auth TO org_service;

-- Enable useful extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";  -- For UUID generation
CREATE EXTENSION IF NOT EXISTS "pg_stat_statements";  -- For query performance monitoring

\echo 'Database initialization complete!'
\echo 'Schemas created: auth, organizations'
\echo 'Service users created with appropriate permissions'
\echo 'Extensions enabled: uuid-ossp, pg_stat_statements'
