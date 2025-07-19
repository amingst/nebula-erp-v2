'use server';

import { OrganizationForm } from '@/components/forms/organization-form';
import DashboardLayout from '@/components/layouts/dashboard-layout';
import { Card, CardContent, CardHeader } from '@/components/ui/card';

export default async function NewOrganizationPage() {
	return (
		<DashboardLayout>
			<Card className='max-w-2xl mx-auto mt-10'>
				<CardHeader>Create a New Organization</CardHeader>
				<CardContent>
					<OrganizationForm />
				</CardContent>
			</Card>
		</DashboardLayout>
	);
}
