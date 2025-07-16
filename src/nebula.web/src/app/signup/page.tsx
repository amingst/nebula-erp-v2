import AuthLayout from '@/components/layouts/auth-layout';
import { SignupForm } from '@/components/signup-form-multistep';

export default function SignupPage() {
	return (
		<AuthLayout>
			<SignupForm />
		</AuthLayout>
	);
}
