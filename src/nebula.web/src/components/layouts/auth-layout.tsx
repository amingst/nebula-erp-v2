import { cn } from '@/lib/utils';
import { ModeToggle } from '@/components/mode-toggle';

interface AuthLayoutProps {
	children: React.ReactNode;
	className?: string;
}

export default function AuthLayout({ children, className }: AuthLayoutProps) {
	return (
		<div className='min-h-screen bg-background flex flex-col justify-center py-12 sm:px-6 lg:px-8'>
			<div className='absolute top-4 right-4'>
				<ModeToggle />
			</div>

			<div className='sm:mx-auto sm:w-full sm:max-w-md'>
				<div className='text-center'>
					<h1 className='text-3xl font-bold text-foreground mb-2'>
						Nebula ERP
					</h1>
					<p className='text-sm text-muted-foreground'>
						Enterprise Resource Planning System
					</p>
				</div>
			</div>

			<div className='mt-8 sm:mx-auto sm:w-full sm:max-w-md'>
				<div
					className={cn(
						'bg-card py-8 px-4 shadow sm:rounded-lg sm:px-10 border',
						className
					)}
				>
					{children}
				</div>
			</div>
		</div>
	);
}
