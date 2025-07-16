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
import { loginUser } from '@/lib/actions/auth';
import { Alert, AlertDescription, AlertTitle } from './ui/alert';
import { Terminal } from 'lucide-react';
import FormResult from './form-result';

export function LoginForm({
	className,
	...props
}: React.ComponentProps<'div'>) {
	const [username, setUsername] = useState('');
	const [password, setPassword] = useState('');
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string>('');
	const router = useRouter();

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		setLoading(true);
		setError('');

		try {
			const response = await loginUser(username, password);

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
				setError(response.error || 'Login failed');
			}
		} catch (error) {
			setError(error instanceof Error ? error.message : 'Unknown error');
		} finally {
			setLoading(false);
		}
	};

	const handleTestLogin = async () => {
		setUsername('jdoe');
		setPassword('password');
		setLoading(true);
		setError('');

		try {
			const response = await loginUser('jdoe', 'password');

			if (response.success) {
				localStorage.setItem('nebula_token', response.token || '');

				// Set cookie for middleware authentication
				document.cookie = `auth-token=${
					response.token || 'authenticated'
				}; path=/; max-age=86400; secure; samesite=strict`;

				router.push('/dashboard');
			} else {
				setError(response.error || 'Test login failed');
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
						Welcome back
					</CardTitle>
					<CardDescription className='text-center'>
						Sign in to your Nebula ERP account
					</CardDescription>

					<FormResult error={error} action='login' />
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

						<Button
							type='submit'
							disabled={loading}
							className='w-full'
						>
							{loading ? 'Signing in...' : 'Sign in'}
						</Button>
					</form>

					<div className='relative'>
						<div className='absolute inset-0 flex items-center'>
							<span className='w-full border-t' />
						</div>
						<div className='relative flex justify-center text-xs uppercase'>
							<span className='bg-card px-2 text-muted-foreground'>
								Or continue with
							</span>
						</div>
					</div>

					<Button
						variant='outline'
						onClick={handleTestLogin}
						disabled={loading}
						className='w-full'
					>
						{loading ? 'Testing...' : 'Test Login (jdoe/password)'}
					</Button>

					<div className='relative'>
						<div className='absolute inset-0 flex items-center'>
							<span className='w-full border-t' />
						</div>
						<div className='relative flex justify-center text-xs uppercase'>
							<span className='bg-card px-2 text-muted-foreground'>
								Don't have an account?
							</span>
						</div>
					</div>

					<Button
						variant='outline'
						onClick={() => router.push('/signup')}
						className='w-full'
					>
						Create Account
					</Button>
				</CardContent>
			</Card>
		</div>
	);
}
