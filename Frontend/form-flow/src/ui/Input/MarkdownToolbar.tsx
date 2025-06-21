import React from "react";

interface ToolbarProps {
	onFormat: (command: string) => void;
	className?: string;
}

export const MarkdownToolbar: React.FC<ToolbarProps> = ({
	onFormat,
	className = "",
}) => {
	return (
		<div
			className={`toolbar flex items-center gap-1 mt-2 p-2 bg-background border border-border rounded shadow-sm ${className}`}
		>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("bold")}
				className="p-2 hover:bg-surface rounded text-sm font-bold text-text transition-colors"
				title="Bold (Ctrl+B)"
			>
				B
			</button>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("italic")}
				className="p-2 hover:bg-surface rounded text-sm italic text-text transition-colors"
				title="Italic (Ctrl+I)"
			>
				I
			</button>
			<button
				onMouseDown={(e) => e.preventDefault()}
				onClick={() => onFormat("underline")}
				className="p-2 hover:bg-surface rounded text-sm underline text-text transition-colors"
				title="Underline (Ctrl+U)"
			>
				U
			</button>
		</div>
	);
};
