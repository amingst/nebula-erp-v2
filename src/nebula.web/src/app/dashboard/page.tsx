'use server';

import DashboardLayout from '@/components/layouts/dashboard-layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { getOrganizationsForUser } from '@/lib/actions/orgs';
import { ArrowRight } from 'lucide-react';
import Link from 'next/link';

export default async function UserOrganizationsPage() {
	const { Records: orgs } = await getOrganizationsForUser();
	const hasOrgs = orgs && orgs.length > 0;

	return (
		<DashboardLayout>
			<div className='space-y-6'>
				<h1 className='text-3xl font-bold'>Your Organizations</h1>
				{!hasOrgs ? (
					<p className='text-muted-foreground'>
						You are not part of any organizations yet. Please create
						or join an organization to get started.
					</p>
				) : (
					<div className='grid gap-4 md:grid-cols-2 lg:grid-cols-3'>
						{orgs.map((org) => (
							<Card key={org.OrganizationId} className='w-full'>
								<CardHeader className='flex flex-row items-center justify-between space-y-0 pb-4'>
									<div className='space-y-1'>
										<CardTitle className='text-xl'>
											{org.OrganizationName}
										</CardTitle>
										<p className='text-sm text-muted-foreground'>
											{org.EmployeeIds?.length || 0}{' '}
											employees
										</p>
									</div>
									<Button asChild variant='ghost' size='sm'>
										<Link
											href={`/dashboard/organizations/${org.OrganizationId}`}
										>
											<ArrowRight className='h-4 w-4' />
										</Link>
									</Button>
								</CardHeader>
							</Card>
						))}
					</div>
				)}
			</div>
		</DashboardLayout>
	);
}
