// ConnectRPC client configuration for Nebula ERP
// @ts-ignore - @bufbuild/connect-node doesn't have complete type declarations
import { createGrpcTransport } from '@bufbuild/connect-node';

// Create transport for connecting to the backend using gRPC
export const transport = createGrpcTransport({
	baseUrl: 'http://localhost:7001', // Adjust this URL to match your backend server
	httpVersion: '2', // gRPC requires HTTP/2
});

// Export base URL for reference
export const API_BASE_URL = 'http://localhost:7001';
