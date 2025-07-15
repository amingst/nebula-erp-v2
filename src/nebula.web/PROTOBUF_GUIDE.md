# Protobuf Integration Guide

This guide explains how to use the generated TypeScript definitions from the Nebula protocol buffers in the frontend application.

## Setup

The protobuf TypeScript definitions are automatically generated from the `.proto` files in `src/Nebula.Fragments/Protos/` and are copied to `src/lib/protos/` for use in the frontend application.

### File Structure

```
src/
└── lib/
    └── protos/
        ├── index.ts                    # Main exports file
        └── Protos/
            └── Nebula/
                └── Services/
                    └── Fragments/
                        ├── Accounting/
                        ├── Authentication/
                        ├── HR/
                        ├── Inventory/
                        ├── Organizations/
                        └── Shared/
```

### Import Usage

```typescript
// Standard imports from the lib directory
import { OrganizationRecord, UserPublicRecord } from '@/lib/protos/index';
import { Timestamp } from '@bufbuild/protobuf';
```

### Copying Files

When protobuf files are updated, copy them to the frontend:

```bash
# From the nebula.web directory
npm run copy-protos
```

Or run the script directly:

```bash
./copy-protos.sh
```

## Development Workflow

1. **Generate TypeScript from Protobuf Files**

    ```bash
    # From src/Nebula.Fragments directory
    ./generate-all-ts.sh
    ```

2. **Copy Generated Files to Frontend**

    ```bash
    # From src/nebula.web directory
    npm run copy-protos
    ```

3. **Use in Components**

    ```typescript
    import { OrganizationRecord } from '@/lib/protos/index';

    const org = new OrganizationRecord({
    	id: '123',
    	name: 'My Organization',
    });
    ```

## Available Types

### Organization Types

-   `OrganizationRecord` - Complete organization information
-   `OrganizationInterface` - Organization service interface

### User Types

-   `UserPublicRecord` - Public user information
-   `UserPrivateRecord` - Private user information (roles, email)
-   `UserServerRecord` - Server-side user data (password hashes)

### Employee Types

-   `EmployeeRecord` - Employee information
-   `EmployeeInterface` - Employee service interface

### Inventory Types

-   `ProductRecord` - Product information
-   `BatchRecord` - Batch tracking
-   `StockRecord` - Stock levels
-   `LocationRecord` - Warehouse locations
-   `SupplierRecord` - Supplier information

### Accounting Types

-   `AccountRecord` - Chart of accounts
-   `TransactionRecord` - Financial transactions
-   `JournalEntryRecord` - Journal entries

## Usage Examples

### Creating Records

```typescript
import { OrganizationRecord, UserPublicRecord } from '@nebula/protos';
import { Timestamp } from '@bufbuild/protobuf';

// Helper for creating timestamps
const createTimestamp = (date: Date): Timestamp => {
	return Timestamp.fromDate(date);
};

// Create an organization
const organization = new OrganizationRecord({
	OrganizationId: 'org-123',
	OrganizationName: 'My Company',
	EmployeeIds: ['emp-1', 'emp-2'],
	CustomerIds: ['cust-1'],
	OwnerId: 'user-123',
	CreatedUTC: createTimestamp(new Date()),
	CreatedBy: 'admin',
});

// Create a user
const user = new UserPublicRecord({
	UserId: 'user-456',
	UserName: 'johndoe',
	DisplayName: 'John Doe',
	Identites: ['email:john@example.com'],
	CreatedUTC: createTimestamp(new Date()),
});
```

### Working with Timestamps

Protocol buffer timestamps are different from JavaScript Date objects:

```typescript
import { Timestamp } from '@bufbuild/protobuf';

// Convert Date to Timestamp
const timestamp = Timestamp.fromDate(new Date());

// Convert Timestamp to Date
const date = timestamp.toDate();
```

### Serialization

```typescript
// Convert to JSON
const jsonString = record.toJsonString();

// Convert from JSON
const record = OrganizationRecord.fromJsonString(jsonString);

// Convert to binary
const binaryData = record.toBinary();

// Convert from binary
const record = OrganizationRecord.fromBinary(binaryData);
```

## Regenerating Types

When proto files are updated, regenerate the TypeScript definitions:

```bash
cd src/Nebula.Fragments
./generate-all-ts.sh
```

Or from the ts-gen directory:

```bash
cd src/Nebula.Fragments/ts-gen
npm run build
```

## Development

The generated files are in `src/Nebula.Fragments/ts-gen/gen/` and are ignored by Git. Only the source `.proto` files and generation scripts are tracked.

## Dependencies

The following packages are required:

-   `@bufbuild/protobuf` - Core protobuf runtime
-   `@bufbuild/protoc-gen-es` - TypeScript code generator
-   `@bufbuild/protoc-gen-connect-es` - Connect (gRPC-web) code generator
