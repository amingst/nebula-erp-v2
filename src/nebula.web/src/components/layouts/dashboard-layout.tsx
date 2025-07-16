'use client';

import { useRouter } from 'next/navigation';
import { SidebarProvider, SidebarTrigger } from '@/components/ui/sidebar';
import { AppSidebar } from '@/components/app-sidebar';
import { Separator } from '@/components/ui/separator';
import { Button } from '@/components/ui/button';
import {
	Breadcrumb,
	BreadcrumbItem,
	BreadcrumbLink,
	BreadcrumbList,
	BreadcrumbPage,
	BreadcrumbSeparator,
} from '@/components/ui/breadcrumb';
import { ModeToggle } from '@/components/mode-toggle';

interface DashboardLayoutProps {
	children: React.ReactNode;
	breadcrumbs?: {
		label: string;
		href?: string;
	}[];
	title?: string;
}

export default function DashboardLayout({
	children,
	breadcrumbs = [],
	title,
}: DashboardLayoutProps) {
	const router = useRouter();

	const handleLogout = () => {
		// Clear localStorage
		localStorage.removeItem('nebula_token');

		// Clear auth cookie
		document.cookie =
			'auth-token=; path=/; expires=Thu, 01 Jan 1970 00:00:01 GMT;';

		// Redirect to login
		router.push('/login');
	};

	return (
		<SidebarProvider>
			<AppSidebar />
			<main className='flex flex-1 flex-col'>
				<header className='sticky top-0 z-10 flex h-16 shrink-0 items-center gap-2 bg-background px-4 border-b'>
					<SidebarTrigger className='-ml-1' />
					<Separator orientation='vertical' className='mr-2 h-4' />

					<div className='flex items-center justify-between w-full'>
						<div className='flex items-center gap-2'>
							{breadcrumbs.length > 0 && (
								<Breadcrumb>
									<BreadcrumbList>
										{breadcrumbs.map(
											(breadcrumb, index) => (
												<div
													key={index}
													className='flex items-center'
												>
													{index > 0 && (
														<BreadcrumbSeparator />
													)}
													<BreadcrumbItem>
														{breadcrumb.href ? (
															<BreadcrumbLink
																href={
																	breadcrumb.href
																}
															>
																{
																	breadcrumb.label
																}
															</BreadcrumbLink>
														) : (
															<BreadcrumbPage>
																{
																	breadcrumb.label
																}
															</BreadcrumbPage>
														)}
													</BreadcrumbItem>
												</div>
											)
										)}
									</BreadcrumbList>
								</Breadcrumb>
							)}
						</div>

						<div className='flex items-center gap-2'>
							<ModeToggle />
							<Button
								variant='outline'
								size='sm'
								onClick={handleLogout}
							>
								Logout
							</Button>
						</div>
					</div>
				</header>

				<div className='flex-1 p-4 pt-0'>
					{title && (
						<div className='py-4'>
							<h1 className='text-2xl font-bold tracking-tight'>
								{title}
							</h1>
						</div>
					)}
					{children}
				</div>
			</main>
		</SidebarProvider>
	);
}
