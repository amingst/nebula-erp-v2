'use client';

import { useState, ReactNode } from 'react';
import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import {
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
} from '@/components/ui/card';
import { Check, LucideIcon } from 'lucide-react';

export interface Step {
	title: string;
	description: string;
	icon: LucideIcon;
	content: ReactNode;
}

export interface MultiStepFormProps {
	steps: Step[];
	title: string;
	onStepValidation?: (
		step: number,
		data: any
	) => { isValid: boolean; error?: string };
	onSubmit?: (data: any) => Promise<void>;
	className?: string;
	submitButtonText?: string;
	loadingText?: string;
	isLoading?: boolean;
	error?: string;
	showStepNavigation?: boolean;
	allowBackNavigation?: boolean;
	footer?: ReactNode;
	onStepChange?: (step: number) => void;
	data?: any;
}

export function MultiStepForm({
	steps,
	title,
	onStepValidation,
	onSubmit,
	className,
	submitButtonText = 'Submit',
	loadingText = 'Processing...',
	isLoading = false,
	error,
	showStepNavigation = true,
	allowBackNavigation = true,
	footer,
	onStepChange,
	data,
}: MultiStepFormProps) {
	const [currentStep, setCurrentStep] = useState(1);
	const [stepError, setStepError] = useState<string>('');
	const [loading, setLoading] = useState(false);

	const totalSteps = steps.length;

	const validateCurrentStep = () => {
		setStepError('');

		if (onStepValidation) {
			const validation = onStepValidation(currentStep, data);
			if (!validation.isValid) {
				setStepError(validation.error || 'Validation failed');
				return false;
			}
		}
		return true;
	};

	const handleNext = () => {
		if (validateCurrentStep()) {
			const nextStep = currentStep + 1;
			setCurrentStep(nextStep);
			onStepChange?.(nextStep);
		}
	};

	const handleBack = () => {
		setStepError('');
		const prevStep = currentStep - 1;
		setCurrentStep(prevStep);
		onStepChange?.(prevStep);
	};

	const handleFormSubmit = async () => {
		if (!validateCurrentStep()) {
			return;
		}

		if (onSubmit) {
			setLoading(true);
			try {
				await onSubmit(data);
			} catch (error) {
				setStepError(
					error instanceof Error ? error.message : 'An error occurred'
				);
			} finally {
				setLoading(false);
			}
		}
	};

	const currentStepData = steps[currentStep - 1];

	return (
		<div className={cn('flex flex-col gap-6', className)}>
			<Card className='w-full'>
				<CardHeader className='space-y-1'>
					<CardTitle className='text-2xl text-center'>
						{title}
					</CardTitle>
					<CardDescription className='text-center'>
						Step {currentStep} of {totalSteps}:{' '}
						{currentStepData.title}
					</CardDescription>

					{/* Progress Steps */}
					{showStepNavigation && (
						<div className='flex justify-center mt-6'>
							<div className='flex items-center space-x-4'>
								{steps.map((step, index) => {
									const stepNumber = index + 1;
									const isActive = stepNumber === currentStep;
									const isCompleted =
										stepNumber < currentStep;
									const IconComponent = step.icon;

									return (
										<div
											key={stepNumber}
											className='flex items-center'
										>
											<div
												className={cn(
													'flex items-center justify-center w-10 h-10 rounded-full border-2 transition-colors',
													isCompleted
														? 'bg-primary border-primary text-primary-foreground'
														: isActive
														? 'border-primary text-primary'
														: 'border-muted-foreground text-muted-foreground'
												)}
											>
												{isCompleted ? (
													<Check className='w-5 h-5' />
												) : (
													<IconComponent className='w-5 h-5' />
												)}
											</div>
											{index < steps.length - 1 && (
												<div
													className={cn(
														'w-12 h-0.5 mx-2 transition-colors',
														isCompleted
															? 'bg-primary'
															: 'bg-muted-foreground'
													)}
												/>
											)}
										</div>
									);
								})}
							</div>
						</div>
					)}

					{/* Error Display */}
					{(error || stepError) && (
						<div className='mt-4 p-3 bg-destructive/10 border border-destructive/20 rounded-md'>
							<p className='text-sm text-destructive font-medium'>
								{error || stepError}
							</p>
						</div>
					)}
				</CardHeader>
				<CardContent className='space-y-6'>
					{/* Step Content */}
					<div className='min-h-[200px]'>
						{currentStepData.content}
					</div>

					{/* Navigation Buttons */}
					<div className='flex justify-between'>
						<Button
							variant='outline'
							onClick={handleBack}
							disabled={currentStep === 1 || !allowBackNavigation}
						>
							Back
						</Button>

						{currentStep < totalSteps ? (
							<Button onClick={handleNext}>Next</Button>
						) : (
							<Button
								onClick={handleFormSubmit}
								disabled={loading || isLoading}
							>
								{loading || isLoading
									? loadingText
									: submitButtonText}
							</Button>
						)}
					</div>

					{/* Custom Footer */}
					{footer && currentStep === 1 && (
						<div className='mt-6'>{footer}</div>
					)}
				</CardContent>
			</Card>
		</div>
	);
}
