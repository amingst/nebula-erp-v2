'use server';

import DashboardLayout from '@/components/layouts/dashboard-layout';
import { getOrganizationById } from '@/lib/actions/orgs';

type Props = {
	params: Promise<{ id: string }>;
};

export default async function OrganizationPage({ params }: Props) {
	const { id } = await params;
	const { Record: org } = await getOrganizationById(id);
	if (!org) {
		return <p className='text-red-500'>Organization not found</p>;
	}

	return (
		<DashboardLayout>
			<h1 className='text-3xl font-bold'>{org.OrganizationName}</h1>
		</DashboardLayout>
	);
}
