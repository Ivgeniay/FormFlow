interface LittleSpinnerProps {
	isLoading?: boolean;
}

export const LittleSpinner: React.FC<LittleSpinnerProps> = ({
	isLoading = true,
}) => {
	return (
		<>
			{isLoading && (
				<div className="flex justify-center py-4">
					<div className="animate-spin rounded-full h-6 w-6 border-b-2 border-primary"></div>
				</div>
			)}
		</>
	);
};
