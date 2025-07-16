'use client';
import { Terminal } from 'lucide-react';
import { Alert, AlertTitle, AlertDescription } from './ui/alert';

export default function FormResult({
	error,
	action,
}: {
	error?: string;
	action?: 'login' | 'signup';
}) {
	return (
		<>
			{error && error !== 'No Error' && (
				<Alert variant='destructive'>
					<Terminal />
					{action === 'login' ? (
						<AlertTitle>Error Logging In</AlertTitle>
					) : action === 'signup' ? (
						<AlertTitle>Error Signing Up</AlertTitle>
					) : null}
					<AlertDescription>{error}</AlertDescription>
				</Alert>
			)}
		</>
	);
}
