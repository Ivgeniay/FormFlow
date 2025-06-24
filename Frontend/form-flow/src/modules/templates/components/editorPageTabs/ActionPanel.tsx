import React from "react";

export interface ActionItem {
	id: string;
	icon: string;
	label: string;
	onClick: () => void;
	variant?: "default" | "danger";
}

interface ActionPanelProps {
	visible: boolean;
	actions: ActionItem[];
	position?: "left" | "right" | "center";
	className?: string;
}

export const ActionPanel: React.FC<ActionPanelProps> = ({
	visible,
	actions,
	position = "right",
	className = "",
}) => {
	const positionClasses = {
		left: "left-4",
		right: "right-4",
		center: "left-1/2 transform -translate-x-1/2",
	};

	return (
		<div
			className={`absolute top-1/2 transform -translate-y-1/2 ${positionClasses[position]} z-20 ${className}`}
		>
			<div
				className={`
					bg-surface border border-border rounded-lg shadow-lg flex items-center
					transition-all duration-200 ease-in-out
					${visible ? "opacity-100 scale-100" : "opacity-0 scale-95 pointer-events-none"}
				`}
			>
				{actions.map((action, index) => (
					<button
						key={action.id}
						onClick={action.onClick}
						title={action.label}
						className={`
							p-2 transition-colors
							${index === 0 ? "rounded-l-lg" : ""}
							${index === actions.length - 1 ? "rounded-r-lg" : ""}
							${index > 0 ? "border-l border-border" : ""}
							${
								action.variant === "danger"
									? "text-textMuted hover:text-error hover:bg-error/10"
									: "text-textMuted hover:text-primary hover:bg-primary/10"
							}
						`}
					>
						{action.icon}
					</button>
				))}
			</div>
		</div>
	);
};
