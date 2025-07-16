// Simple test script to test authentication
const { createPromiseClient } = require('@bufbuild/connect');
const { createGrpcTransport } = require('@bufbuild/connect-node');
const {
	UserInterface,
} = require('./src/nebula.web/src/lib/protos/Protos/Nebula/Services/Fragments/Authentication/UserInterface_connect');
const {
	AutheticateUserRequest,
} = require('./src/nebula.web/src/lib/protos/Protos/Nebula/Services/Fragments/Authentication/UserInterface_pb');

async function testAuth() {
	try {
		// Create transport
		const transport = createGrpcTransport({
			baseUrl: 'http://localhost:7001',
			httpVersion: '2',
		});

		// Create client
		const client = createPromiseClient(UserInterface, transport);

		// Create request
		const request = new AutheticateUserRequest({
			UserName: 'testuser',
			Password: 'password123',
		});

		console.log('Testing authentication...');
		const response = await client.authenticateUser(request);

		console.log('Response:', response);

		if (response.Token) {
			console.log(
				'SUCCESS: Token received:',
				response.Token.substring(0, 20) + '...'
			);
		} else {
			console.log('ERROR:', response.Error);
		}
	} catch (error) {
		console.error('Error:', error);
	}
}

testAuth();
