'use server';
import { OrganizationInterface } from '../protos/Protos/Nebula/Services/Fragments/Organizations/OrganizationInterface_connect';
import { createPromiseClient } from '@bufbuild/connect';
import { transport } from '@/lib/connect-client';
import { cookies } from 'next/headers';
import {
	CreateOrganizationRequest,
	CreateOrganizationResponse,
	GetOrganizationRequest,
	GetOrganizationResponse,
	GetOrganizationsForUserRequest,
	GetOrganizationsForUserResponse,
} from '../protos/Protos/Nebula/Services/Fragments/Organizations/OrganizationInterface_pb';
import { OrganizationRecord } from '../protos/Protos/Nebula/Services/Fragments/Organizations/OrganizationRecord_pb';
import { JsonValue } from '@bufbuild/protobuf';

export async function getOrganizationsForUser(): Promise<GetOrganizationsForUserResponse> {
	try {
		const token = await (await cookies()).get('auth-token')?.value;
		const client = createPromiseClient(OrganizationInterface, transport);
		const request = new GetOrganizationsForUserRequest();
		const response = await client.getOrganizationsForUser(request, {
			headers: { Authorization: `Bearer ${token}` },
		});
		console.log('Fetched organizations:', response);
		return response;
	} catch (error) {
		console.error('Error fetching organizations:', error);
		return new GetOrganizationsForUserResponse({
			Error: 'Something went wrong while fetching organizations',
		});
	}
}

export async function createOrganization(
	name: string
): Promise<{ error: string; Record: OrganizationRecord }> {
	try {
		const token = await (await cookies()).get('auth-token')?.value;
		const request = new CreateOrganizationRequest({
			OrganizationName: name,
		});
		const client = createPromiseClient(OrganizationInterface, transport);

		const response = await client.createOrganization(request, {
			headers: { Authorization: `Bearer ${token}` },
		});
		console.log('Created organization:', response);

		return {
			error: response.Error || '',
			Record: response.Record,
		};
	} catch (error) {
		console.error('Error creating organization:', error);
		return {
			error: 'Something went wrong while creating organization',
			Record: {} as OrganizationRecord,
		};
	}
}

export async function getOrganizationById(
	id: string
): Promise<GetOrganizationResponse> {
	try {
		const token = await (await cookies()).get('auth-token')?.value;
		const client = createPromiseClient(OrganizationInterface, transport);
		const request = new GetOrganizationRequest({ OrganizationId: id });
		const response = await client.getOrganization(request, {
			headers: { Authorization: `Bearer ${token}` },
		});
		console.log('Fetched organization:', response);
		return response;
	} catch (error) {
		console.error('Error fetching organization:', error);
		return new GetOrganizationResponse({
			Error: 'Something went wrong while fetching organization',
			Record: {} as OrganizationRecord,
		});
	}
}
