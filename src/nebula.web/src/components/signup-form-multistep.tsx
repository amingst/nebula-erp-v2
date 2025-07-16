'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';
import { registerUser } from '@/lib/actions/auth';
import { User, Mail, Lock } from 'lucide-react';
import { MultiStepForm, Step } from '@/components/ui/multi-step-form';

export function SignupForm({
	className,
	...props
}: React.ComponentProps<'div'>) {
	const [formData, setFormData] = useState({
		username: '',
		email: '',
		displayName: '',
		password: '',
		confirmPassword: '',
	});
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string>('');
	const router = useRouter();

	const updateFormData = (field: string, value: string) => {
		setFormData((prev) => ({ ...prev, [field]: value }));
	};

	const steps: Step[] = [
		{
			title: 'Account Details',
			description: 'Create your username',
			icon: User,
			content: (
				<div className='space-y-4'>
					<div className='space-y-2'>
						<Label htmlFor='username'>Username</Label>
						<Input
							id='username'
							type='text'
							value={formData.username}
							onChange={(e) =>
								updateFormData('username', e.target.value)
							}
							placeholder='Enter your username'
							required
						/>
					</div>
				</div>
			),
		},
		{
			title: 'Profile Information',
			description: 'Tell us about yourself',
			icon: Mail,
			content: (
				<div className='space-y-4'>
					<div className='space-y-2'>
						<Label htmlFor='email'>Email</Label>
						<Input
							id='email'
							type='email'
							value={formData.email}
							onChange={(e) =>
								updateFormData('email', e.target.value)
							}
							placeholder='Enter your email'
							required
						/>
					</div>
					<div className='space-y-2'>
						<Label htmlFor='displayName'>Display Name</Label>
						<Input
							id='displayName'
							type='text'
							value={formData.displayName}
							onChange={(e) =>
								updateFormData('displayName', e.target.value)
							}
							placeholder='Enter your display name'
							required
						/>
					</div>
				</div>
			),
		},
		{
			title: 'Security',
			description: 'Set up your password',
			icon: Lock,
			content: (
				<div className='space-y-4'>
					<div className='space-y-2'>
						<Label htmlFor='password'>Password</Label>
						<Input
							id='password'
							type='password'
							value={formData.password}
							onChange={(e) =>
								updateFormData('password', e.target.value)
							}
							placeholder='Enter your password'
							required
						/>
					</div>
					<div className='space-y-2'>
						<Label htmlFor='confirmPassword'>
							Confirm Password
						</Label>
						<Input
							id='confirmPassword'
							type='password'
							value={formData.confirmPassword}
							onChange={(e) =>
								updateFormData(
									'confirmPassword',
									e.target.value
								)
							}
							placeholder='Confirm your password'
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
				if (!data.username.trim()) {
					return { isValid: false, error: 'Username is required' };
				}
				if (data.username.length < 3) {
					return {
						isValid: false,
						error: 'Username must be at least 3 characters',
					};
				}
				return { isValid: true };
			case 2:
				if (!data.email.trim()) {
					return { isValid: false, error: 'Email is required' };
				}
				if (!data.displayName.trim()) {
					return {
						isValid: false,
						error: 'Display name is required',
					};
				}
				if (data.displayName.length < 2) {
					return {
						isValid: false,
						error: 'Display name must be at least 2 characters',
					};
				}
				return { isValid: true };
			case 3:
				if (!data.password) {
					return { isValid: false, error: 'Password is required' };
				}
				if (data.password.length < 6) {
					return {
						isValid: false,
						error: 'Password must be at least 6 characters',
					};
				}
				if (data.password !== data.confirmPassword) {
					return { isValid: false, error: 'Passwords do not match' };
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
			const response = await registerUser(
				data.username,
				data.email,
				data.displayName,
				data.password
			);

			if (response.success) {
				// Store token in both localStorage and as a cookie for middleware
				localStorage.setItem('nebula_token', response.token || '');

				// Set cookie for middleware authentication
				document.cookie = `auth-token=${
					response.token || 'authenticated'
				}; path=/; max-age=86400; secure; samesite=strict`;

				// Redirect to dashboard
				router.push('/dashboard');
			} else {
				setError(response.error || 'Registration failed');
				throw new Error(response.error || 'Registration failed');
			}
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
						Already have an account?
					</span>
				</div>
			</div>

			<Button
				variant='outline'
				onClick={() => router.push('/login')}
				className='w-full'
			>
				Sign In Instead
			</Button>
		</>
	);

	return (
		<MultiStepForm
			steps={steps}
			title='Create Account'
			onStepValidation={validateStep}
			onSubmit={handleSubmit}
			submitButtonText='Create Account'
			loadingText='Creating Account...'
			isLoading={loading}
			error={error}
			footer={footer}
			data={formData}
			className={className}
		/>
	);
}
