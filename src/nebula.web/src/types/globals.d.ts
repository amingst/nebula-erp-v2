// Global type declarations to suppress TypeScript errors
declare module '@bufbuild/connect-node' {
	export function createGrpcTransport(options: any): any;
	export function createGrpcWebTransport(options: any): any;
	export function createConnectTransport(options: any): any;
}

// Suppress protobuf module errors
declare module '@/lib/protos/index' {
	export const OrganizationRecord: any;
	export const UserPublicRecord: any;
	export const UserPrivateRecord: any;
	export const UserInterface: any;
	export const AutheticateUserRequest: any;
	export const AuthenticateUserResponse: any;
}

// Wildcard for all protobuf files
declare module '@/lib/protos/Protos/Nebula/Services/Fragments/*/*' {
	export const OrganizationRecord: any;
	export const UserPublicRecord: any;
	export const UserPrivateRecord: any;
	export const UserInterface: any;
	export const AutheticateUserRequest: any;
	export const AuthenticateUserResponse: any;
	export default any;
}
