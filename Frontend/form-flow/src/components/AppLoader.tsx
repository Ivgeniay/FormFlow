import React from "react";

interface AppLoaderProps {
	isVisible: boolean;
	loadingText?: string;
	showError?: boolean;
	onRetry?: () => void;
}

export const AppLoader: React.FC<AppLoaderProps> = ({
	isVisible,
	loadingText,
	showError = false,
	onRetry,
}) => {
	if (!isVisible) {
		return null;
	}

	return (
		<div className="fixed inset-0 bg-background flex items-center justify-center z-50">
			<div className="flex flex-col items-center">
				<h1 className="text-4xl font-bold text-primary mb-8">FormFlow</h1>

				{showError ? (
					<div className="flex flex-col items-center">
						<div className="text-error mb-4">
							<svg
								className="w-12 h-12"
								fill="none"
								stroke="currentColor"
								viewBox="0 0 24 24"
							>
								<path
									strokeLinecap="round"
									strokeLinejoin="round"
									strokeWidth={2}
									d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
								/>
							</svg>
						</div>
						{onRetry && (
							<button
								onClick={onRetry}
								className="px-6 py-3 bg-error text-white rounded-lg hover:opacity-90 transition-colors"
							>
								Retry
							</button>
						)}
					</div>
				) : (
					<div className="flex flex-col items-center">
						<div className="animate-spin rounded-full h-12 w-12 border-4 border-border border-t-primary mb-4"></div>
						<div className="h-6 flex items-center">
							{loadingText && (
								<p className="text-textMuted text-center max-w-sm">{loadingText}</p>
							)}
						</div>
					</div>
				)}
			</div>
		</div>
	);
};
