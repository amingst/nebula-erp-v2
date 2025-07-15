// Simple test to verify protobuf imports are working
import { OrganizationRecord, UserPublicRecord } from '@nebula/protos';
import { Timestamp } from '@bufbuild/protobuf';

console.log('Testing protobuf imports...');

// Test creating an OrganizationRecord
const testOrg = new OrganizationRecord({
	OrganizationId: 'test-org',
	OrganizationName: 'Test Organization',
});

console.log('OrganizationRecord created:', testOrg);

// Test creating a UserPublicRecord
const testUser = new UserPublicRecord({
	UserId: 'test-user',
	UserName: 'testuser',
	DisplayName: 'Test User',
});

console.log('UserPublicRecord created:', testUser);

// Test Timestamp functionality
const now = Timestamp.fromDate(new Date());
console.log('Timestamp created:', now);

export {}; // Make this a module
