interface UserAvatarProps {
	className?: string;
	size?: number;
	color?: string;
	picture?: string;
	literal?: string;
	mode?: "picture" | "literal";
}

export const UserAvanar: React.FC<UserAvatarProps> = ({
	className,
	size = 8,
	color = "text-text",
	picture,
	literal = "AVATAR",
	mode = "picture",
}) => {
	const isLitera = mode === `literal` && literal !== undefined;
	const isPicture = mode === `picture` && picture !== undefined;
	const isSvg = mode === `picture`;

	return (
		<div
			className={`w-${size} h-${size} ${className} rounded-full bg-surface border border-border overflow-hidden flex items-center justify-center`}
		>
			{isLitera && (
				<div
					className={`w-${size} h-${size} bg-primary rounded-full flex items-center justify-center text-text text-2xl font-bold`}
				>
					{`${literal}`}
				</div>
			)}

			{isPicture && (
				<img
					src={picture}
					alt="User avatar"
					className="w-full h-full object-cover"
				/>
			)}

			{isSvg && (
				<svg
					className={`w-${size - 2} h-${size - 2}  ${color}`}
					fill="none"
					stroke="currentColor"
					viewBox="0 0 24 24"
				>
					<path
						strokeLinecap="round"
						strokeLinejoin="round"
						strokeWidth={2}
						d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
					/>
				</svg>
			)}
		</div>
	);
};
