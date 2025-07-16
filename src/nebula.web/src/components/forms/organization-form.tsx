'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';
import { MultiStepForm, Step } from '@/components/ui/multi-step-form';
import { User } from 'lucide-react';
// import your gRPC or API action here
// import { createOrganization } from '@/lib/actions/organization';

export function OrganizationForm({
	className,
	...props
}: React.ComponentProps<'div'>) {
	const [formData, setFormData] = useState({
		organizationName: '',
	});
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string>('');
	const router = useRouter();

	const updateFormData = (field: string, value: string) => {
		setFormData((prev) => ({ ...prev, [field]: value }));
	};

	const steps: Step[] = [
		{
			title: 'Organization Details',
			description: 'Enter your organization name',
			icon: User,
			content: (
				<div className='space-y-4'>
					<div className='space-y-2'>
						<Label htmlFor='organizationName'>
							Organization Name
						</Label>
						<Input
							id='organizationName'
							type='text'
							value={formData.organizationName}
							onChange={(e) =>
								updateFormData(
									'organizationName',
									e.target.value
								)
							}
							placeholder='Enter your organization name'
							required
						/>
					</div>
				</div>
			),
		},
	];

	const validateStep = (step: number, data: any) => {
		switch (step) {
			case 1:
				if (!data.organizationName.trim()) {
					return {
						isValid: false,
						error: 'Organization name is required',
					};
				}
				if (data.organizationName.length < 2) {
					return {
						isValid: false,
						error: 'Organization name must be at least 2 characters',
					};
				}
				return { isValid: true };
			default:
				return { isValid: true };
		}
	};

	const handleSubmit = async (data: any): Promise<void> => {
		setLoading(true);
		setError('');
		try {
			// Replace with your actual gRPC/API call
			// const response = await createOrganization({ OrganizationName: data.organizationName });
			// if (response.success) {
			//   router.push('/dashboard/organizations');
			// } else {
			//   setError(response.error || 'Failed to create organization');
			//   throw new Error(response.error || 'Failed to create organization');
			// }
			// Demo: simulate success
			setTimeout(() => {
				router.push('/dashboard/organizations');
			}, 800);
		} catch (error) {
			const errorMessage =
				error instanceof Error ? error.message : 'Unknown error';
			setError(errorMessage);
			throw error;
		} finally {
			setLoading(false);
		}
	};

	const footer = (
		<>
			<div className='relative'>
				<div className='absolute inset-0 flex items-center'>
					<span className='w-full border-t' />
				</div>
				<div className='relative flex justify-center text-xs uppercase'>
					<span className='bg-card px-2 text-muted-foreground'>
						Want to manage another organization?
					</span>
				</div>
			</div>
			<Button
				variant='outline'
				onClick={() => router.push('/dashboard/organizations')}
				className='w-full'
			>
				Back to Organizations
			</Button>
		</>
	);

	return (
		<MultiStepForm
			steps={steps}
			title='Create Organization'
			onStepValidation={validateStep}
			onSubmit={handleSubmit}
			submitButtonText='Create Organization'
			loadingText='Creating Organization...'
			isLoading={loading}
			error={error}
			footer={footer}
			data={formData}
			className={className}
		/>
	);
}
