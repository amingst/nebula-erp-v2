'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import {
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
} from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { registerUser } from '@/lib/actions/auth';
import { Alert, AlertDescription, AlertTitle } from './ui/alert';
import { Terminal } from 'lucide-react';

export function SignupForm({
	className,
	...props
}: React.ComponentProps<'div'>) {
	const [username, setUsername] = useState('');
	const [email, setEmail] = useState('');
	const [displayName, setDisplayName] = useState('');
	const [password, setPassword] = useState('');
	const [confirmPassword, setConfirmPassword] = useState('');
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string>('');
	const router = useRouter();

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		setLoading(true);
		setError('');

		// Validate passwords match
		if (password !== confirmPassword) {
			setError('Passwords do not match');
			setLoading(false);
			return;
		}

		// Basic validation
		if (!username || !email || !displayName || !password) {
			setError('All fields are required');
			setLoading(false);
			return;
		}

		try {
			const response = await registerUser(
				username,
				email,
				displayName,
				password
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
			}
		} catch (error) {
			setError(error instanceof Error ? error.message : 'Unknown error');
		} finally {
			setLoading(false);
		}
	};

	return (
		<div className={cn('flex flex-col gap-6', className)} {...props}>
			<Card className='w-full'>
				<CardHeader className='space-y-1'>
					<CardTitle className='text-2xl text-center'>
						Create Account
					</CardTitle>
					<CardDescription className='text-center'>
						Create your Nebula ERP account
					</CardDescription>

					{error && error !== 'No Error' && (
						<Alert variant='destructive'>
							<Terminal />
							<AlertTitle>Error Creating Account</AlertTitle>
							<AlertDescription>{error}</AlertDescription>
						</Alert>
					)}
				</CardHeader>
				<CardContent className='space-y-4'>
					<form onSubmit={handleSubmit} className='space-y-4'>
						<div className='space-y-2'>
							<Label htmlFor='username'>Username</Label>
							<Input
								id='username'
								type='text'
								value={username}
								onChange={(e) => setUsername(e.target.value)}
								placeholder='Enter your username'
								required
							/>
						</div>

						<div className='space-y-2'>
							<Label htmlFor='email'>Email</Label>
							<Input
								id='email'
								type='email'
								value={email}
								onChange={(e) => setEmail(e.target.value)}
								placeholder='Enter your email'
								required
							/>
						</div>

						<div className='space-y-2'>
							<Label htmlFor='displayName'>Display Name</Label>
							<Input
								id='displayName'
								type='text'
								value={displayName}
								onChange={(e) => setDisplayName(e.target.value)}
								placeholder='Enter your display name'
								required
							/>
						</div>

						<div className='space-y-2'>
							<Label htmlFor='password'>Password</Label>
							<Input
								id='password'
								type='password'
								value={password}
								onChange={(e) => setPassword(e.target.value)}
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
								value={confirmPassword}
								onChange={(e) =>
									setConfirmPassword(e.target.value)
								}
								placeholder='Confirm your password'
								required
							/>
						</div>

						<Button
							type='submit'
							disabled={loading}
							className='w-full'
						>
							{loading ? 'Creating Account...' : 'Create Account'}
						</Button>
					</form>

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
				</CardContent>
			</Card>
		</div>
	);
}
