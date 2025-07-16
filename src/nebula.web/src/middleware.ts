import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
	// Check if the user is accessing a protected route
	if (request.nextUrl.pathname.startsWith('/dashboard')) {
		// Check for authentication token in cookies
		const token = request.cookies.get('auth-token');

		if (!token) {
			// Redirect to login if no token is found
			return NextResponse.redirect(new URL('/login', request.url));
		}

		// TODO: In a real app, you'd want to verify the token here
		// For now, we'll just check if it exists
	}

	// If user is already logged in and tries to access login or signup page, redirect to dashboard
	if (
		request.nextUrl.pathname === '/login' ||
		request.nextUrl.pathname === '/signup'
	) {
		const token = request.cookies.get('auth-token');

		if (token) {
			return NextResponse.redirect(new URL('/dashboard', request.url));
		}
	}

	return NextResponse.next();
}

export const config = {
	matcher: ['/dashboard/:path*', '/login', '/signup'],
};
