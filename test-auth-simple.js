// Simple auth test with native fetch
async function testAuth() {
    const baseUrl = 'http://localhost:8001';
    
    try {
        // Step 1: Register the testuser
        console.log('Step 1: Registering testuser...');
        const registerResponse = await fetch(`${baseUrl}/api/v1/auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                UserName: 'testuser',
                Email: 'test@example.com',
                DisplayName: 'Test User',
                Password: 'password123'
            })
        });
        
        const registerData = await registerResponse.json();
        console.log('Register response:', registerData);
        
        if (registerData.Token) {
            console.log('✅ Registration successful! Token received:', registerData.Token.substring(0, 50) + '...');
        } else if (registerData.Error === 'Username already taken') {
            console.log('⚠️  User already exists, proceeding to login...');
        } else {
            console.log('❌ Registration failed:', registerData.Error);
        }
        
        // Step 2: Login with the testuser
        console.log('\nStep 2: Logging in with testuser...');
        const loginResponse = await fetch(`${baseUrl}/api/v1/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                UserName: 'testuser',
                Password: 'password123'
            })
        });
        
        const loginData = await loginResponse.json();
        console.log('Login response:', loginData);
        
        if (loginData.Token) {
            console.log('✅ Login successful! Token received:', loginData.Token.substring(0, 50) + '...');
            
            // Step 3: Test the token by making an authenticated request
            console.log('\nStep 3: Testing token with Organizations endpoint...');
            const orgsResponse = await fetch(`${baseUrl}/api/v1/organizations`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${loginData.Token}`
                },
                body: JSON.stringify({
                    OrganizationName: 'Test Organization'
                })
            });
            
            const orgsData = await orgsResponse.json();
            console.log('Organizations response:', orgsData);
            
            if (orgsResponse.ok) {
                console.log('✅ Token works! Successfully accessed authenticated endpoint');
            } else {
                console.log('❌ Token failed:', orgsResponse.status, orgsData);
            }
            
        } else {
            console.log('❌ Login failed:', loginData.Error);
        }
        
    } catch (error) {
        console.error('Error:', error);
    }
}

testAuth();
