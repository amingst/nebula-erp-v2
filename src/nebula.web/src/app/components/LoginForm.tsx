'use client';

import { useState } from 'react';
import { loginUser, testLogin } from '@/lib/actions/auth';

export default function LoginForm() {
	const [username, setUsername] = useState('');
	const [password, setPassword] = useState('');
	const [loading, setLoading] = useState(false);
	const [result, setResult] = useState<string>('');

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		setLoading(true);
		setResult('');

		try {
			const response = await loginUser(username, password);

			if (response.success) {
				setResult(`✅ Login successful! Token: ${response.token}`);
			} else {
				setResult(`❌ Login failed: ${response.error}`);
			}
		} catch (error) {
			setResult(
				`❌ Error: ${
					error instanceof Error ? error.message : 'Unknown error'
				}`
			);
		} finally {
			setLoading(false);
		}
	};

	const handleTestLogin = async () => {
		setLoading(true);
		setResult('');

		try {
			const response = await testLogin();

			if (response.success) {
				setResult(`✅ Test login successful! Token: ${response.token}`);
			} else {
				setResult(`❌ Test login failed: ${response.error}`);
			}
		} catch (error) {
			setResult(
				`❌ Test error: ${
					error instanceof Error ? error.message : 'Unknown error'
				}`
			);
		} finally {
			setLoading(false);
		}
	};

	return (
		<div className='max-w-md mx-auto mt-8 p-6 bg-white rounded-lg shadow-md'>
			<h2 className='text-2xl font-bold mb-6 text-center'>
				Login to Nebula ERP
			</h2>

			<form onSubmit={handleSubmit} className='space-y-4'>
				<div>
					<label
						htmlFor='username'
						className='block text-sm font-medium text-gray-700'
					>
						Username
					</label>
					<input
						id='username'
						type='text'
						value={username}
						onChange={(e) => setUsername(e.target.value)}
						className='mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500'
						required
					/>
				</div>

				<div>
					<label
						htmlFor='password'
						className='block text-sm font-medium text-gray-700'
					>
						Password
					</label>
					<input
						id='password'
						type='password'
						value={password}
						onChange={(e) => setPassword(e.target.value)}
						className='mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500'
						required
					/>
				</div>

				<button
					type='submit'
					disabled={loading}
					className='w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50'
				>
					{loading ? 'Logging in...' : 'Login'}
				</button>
			</form>

			<div className='mt-4'>
				<button
					onClick={handleTestLogin}
					disabled={loading}
					className='w-full flex justify-center py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-gray-100 hover:bg-gray-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500 disabled:opacity-50'
				>
					{loading ? 'Testing...' : 'Test Login (jdoe/password)'}
				</button>
			</div>

			{result && (
				<div className='mt-4 p-3 bg-gray-100 rounded-md'>
					<pre className='text-sm text-gray-800 whitespace-pre-wrap'>
						{result}
					</pre>
				</div>
			)}
		</div>
	);
}
