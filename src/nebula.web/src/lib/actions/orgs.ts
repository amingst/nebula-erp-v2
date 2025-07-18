import { OrganizationInterface } from "../protos/Protos/Nebula/Services/Fragments/Organizations/OrganizationInterface_connect";
import { createPromiseClient } from '@bufbuild/connect';
import { transport } from '@/lib/connect-client';
import { cookies } from "next/headers";
import { GetOrganizationsForUserRequest, GetOrganizationsForUserResponse } from "../protos/Protos/Nebula/Services/Fragments/Organizations/OrganizationInterface_pb";

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