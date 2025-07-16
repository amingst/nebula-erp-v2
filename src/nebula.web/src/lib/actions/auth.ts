'use server';

import { createPromiseClient } from '@bufbuild/connect';
import { transport } from '@/lib/connect-client';
import { UserInterface } from '@/lib/protos/Protos/Nebula/Services/Fragments/Authentication/UserInterface_connect';
import {
	AutheticateUserRequest,
	AuthenticateUserResponse,
	RegisterUserRequest,
} from '@/lib/protos/Protos/Nebula/Services/Fragments/Authentication/UserInterface_pb';

export async function loginUser(
	username: string,
	password: string
): Promise<{
	success: boolean;
	token?: string;
	error?: string;
}> {
	try {
		console.log(`Attempting to authenticate user: ${username}`);

		// Create the login request
		const request = new AutheticateUserRequest({
			UserName: username,
			Password: password,
		});

		// Let's try using the createPromiseClient approach (v0.13.0)
		// but with our generated service descriptor
		const client = createPromiseClient(UserInterface, transport);

		// Based on the documentation, this should work
		const response = await client.authenticateUser(request);

		// Check if authentication was successful
		if (response.Error && response.Error !== 'No Error') {
			console.error(`Authentication failed: ${response.Error}`);
			return {
				success: false,
				error: response.Error,
			};
		}

		if (response.Token) {
			console.log(`Authentication successful for user: ${username}`);
			return {
				success: true,
				token: response.Token,
			};
		}

		return {
			success: false,
			error: 'No token received',
		};
	} catch (error) {
		console.error('Authentication error:', error);
		return {
			success: false,
			error:
				error instanceof Error
					? error.message
					: 'Unknown error occurred',
		};
	}
}

export async function registerUser(
	username: string,
	email: string,
	displayName: string,
	password: string
): Promise<{
	success: boolean;
	token?: string;
	error?: string;
}> {
	try {
		console.log(`Attempting to register user: ${username}`);

		// Create the register request
		const request = new RegisterUserRequest({
			UserName: username,
			Email: email,
			DisplayName: displayName,
			Password: password,
		});

		// Create the client
		const client = createPromiseClient(UserInterface, transport);

		// Call the register endpoint
		const response = await client.registerUser(request);

		// Check if registration was successful
		if (response.Error && response.Error !== 'No Error') {
			console.error(`Registration failed: ${response.Error}`);
			return {
				success: false,
				error: response.Error,
			};
		}

		if (response.Token) {
			console.log(`Registration successful for user: ${username}`);
			return {
				success: true,
				token: response.Token,
			};
		}

		return {
			success: false,
			error: 'No token received',
		};
	} catch (error) {
		console.error('Registration error:', error);
		return {
			success: false,
			error:
				error instanceof Error
					? error.message
					: 'Unknown error occurred',
		};
	}
}

// Test function with the requested test data
export async function testLogin() {
	return await loginUser('jdoe', 'password');
}
