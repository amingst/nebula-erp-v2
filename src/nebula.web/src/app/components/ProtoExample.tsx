'use client';

import { OrganizationRecord } from '@/lib/protos/Protos/Nebula/Services/Fragments/Organizations/OrganizationRecord_pb';
import {
	UserPublicRecord,
	UserPrivateRecord,
} from '@/lib/protos/Protos/Nebula/Services/Fragments/Authentication/UserRecord_pb';
import { Timestamp } from '@bufbuild/protobuf';

interface ProtoExampleProps {
	// This component demonstrates how to use the imported protobuf types
}

export default function ProtoExample() {
	// Helper function to create a Timestamp from a Date
	const createTimestamp = (date: Date): Timestamp => {
		return Timestamp.fromDate(date);
	};

	// Example of creating a new OrganizationRecord
	const createOrganization = () => {
		const now = new Date();
		const org = new OrganizationRecord({
			OrganizationId: 'org-123',
			OrganizationName: 'Test Organization',
			EmployeeIds: ['emp-1', 'emp-2'],
			CustomerIds: ['cust-1', 'cust-2'],
			OwnerId: 'owner-1',
			CreatedUTC: createTimestamp(now),
			CreatedBy: 'user-123',
			LastModifiedUTC: createTimestamp(now),
			LastModifiedBy: 'user-123',
		});

		console.log('Created organization:', org);
		return org;
	};

	// Example of creating a new UserPublicRecord
	const createUser = () => {
		const now = new Date();
		const user = new UserPublicRecord({
			UserId: 'user-456',
			UserName: 'johndoe',
			DisplayName: 'John Doe',
			Identites: ['email:john@example.com'],
			CreatedUTC: createTimestamp(now),
			LastModifiedUTC: createTimestamp(now),
			LastLoginUTC: createTimestamp(now),
		});

		console.log('Created user:', user);
		return user;
	};

	// Example of creating a UserPrivateRecord
	const createPrivateUser = () => {
		const user = new UserPrivateRecord({
			Email: 'john@example.com',
			Roles: ['user', 'admin'],
			OrganizationRoles: [],
		});

		console.log('Created private user:', user);
		return user;
	};

	return (
		<div className='p-4 border rounded-lg bg-gray-50 dark:bg-gray-800'>
			<h2 className='text-xl font-semibold mb-4'>
				Protobuf Types Example
			</h2>
			<div className='space-y-4'>
				<div>
					<h3 className='font-medium mb-2'>
						Available Protobuf Types:
					</h3>
					<ul className='text-sm space-y-1 text-gray-600 dark:text-gray-400'>
						<li>• OrganizationRecord</li>
						<li>• UserPublicRecord</li>
						<li>• UserPrivateRecord</li>
						<li>• EmployeeRecord</li>
						<li>• AccountRecord</li>
						<li>• And many more...</li>
					</ul>
				</div>

				<div className='space-x-2'>
					<button
						onClick={createOrganization}
						className='px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600'
					>
						Create Organization
					</button>
					<button
						onClick={createUser}
						className='px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600'
					>
						Create Public User
					</button>
					<button
						onClick={createPrivateUser}
						className='px-4 py-2 bg-purple-500 text-white rounded hover:bg-purple-600'
					>
						Create Private User
					</button>
				</div>

				<div className='text-sm text-gray-600 dark:text-gray-400'>
					<p>Check the browser console to see the created objects.</p>
				</div>
			</div>
		</div>
	);
}
