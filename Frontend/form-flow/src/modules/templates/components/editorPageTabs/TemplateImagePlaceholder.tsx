import React from "react";

interface TemplatePlaceholderProps {
	className?: string;
}

export const TemplatePlaceholder: React.FC<TemplatePlaceholderProps> = ({
	className = "",
}) => {
	return (
		<div
			className={`w-full h-full bg-gradient-to-b from-primary to-primary/70 flex items-center justify-center ${className}`}
		>
			<svg
				className="w-16 h-16 text-white/70"
				fill="none"
				stroke="currentColor"
				viewBox="0 0 24 24"
			>
				<path
					strokeLinecap="round"
					strokeLinejoin="round"
					strokeWidth={1.5}
					d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
				/>
			</svg>
		</div>
	);
};
